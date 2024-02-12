@description('Location where all resources will be deployed. This value defaults to the **East US** region.')
@allowed([
  'southcentralus'
  'eastus'
  'westeurope'
])
param location string = 'eastus'

@description('''
Unique name for the deployed services below. Max length 15 characters, alphanumeric only:
- Azure Cosmos DB for MongoDB vCore
- Azure OpenAI
- Azure App Service

The name defaults to a unique string generated from the resource group identifier.
''')
@maxLength(15)
param name string = uniqueString(resourceGroup().id)

@description('Specifies the SKU for the Azure App Service plan. Defaults to **B1**')
@allowed([
  'B1'
  'S1'
])
param appServiceSku string = 'B1'

@description('Specifies the SKU for the Azure OpenAI resource. Defaults to **S0**')
@allowed([
  'S0'
])
param openAiSku string = 'S0'

@description('MongoDB vCore user Name. No dashes.')
param mongoDbUserName string

@description('MongoDB vCore password. 8-256 characters, 3 of the following: lower case, upper case, numeric, symbol.')
@minLength(8)
@maxLength(256)
@secure()
param mongoDbPassword string


var openAiSettings = {
  name: '${name}-openai'
  sku: openAiSku
  maxConversationTokens: '100'
  maxCompletionTokens: '500'
  completionsModel: {
    name: 'gpt-35-turbo'
    version: '0301'
    deployment: {
      name: 'completions'
    }
  }
  embeddingsModel: {
    name: 'text-embedding-ada-002'
    version: '2'
    deployment: {
      name: 'embeddings'
    }
  }
}

var mongovCoreSettings = {
  mongoClusterName: '${name}-mongo'
  mongoClusterLogin: mongoDbUserName
  mongoClusterPassword: mongoDbPassword
}

var appServiceSettings = {
  plan: {
    name: '${name}-web-plan'
    sku: appServiceSku
  }
  web: {
    name: '${name}-web'
  }
  api: {
    name: '${name}-api'
  }
}

resource mongoCluster 'Microsoft.DocumentDB/mongoClusters@2023-03-01-preview' = {
  name: mongovCoreSettings.mongoClusterName
  location: location
  properties: {
    administratorLogin: mongovCoreSettings.mongoClusterLogin
    administratorLoginPassword: mongovCoreSettings.mongoClusterPassword
    serverVersion: '5.0'
    nodeGroupSpecs: [
      {
        kind: 'Shard'
        sku: 'M30'
        diskSizeGB: 128
        enableHa: false
        nodeCount: 1
      }
    ]
  }
}

resource mongoFirewallRulesAllowAzure 'Microsoft.DocumentDB/mongoClusters/firewallRules@2023-03-01-preview' = {
  parent: mongoCluster
  name: 'allowAzure'
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}

resource mongoFirewallRulesAllowAll 'Microsoft.DocumentDB/mongoClusters/firewallRules@2023-03-01-preview' = {
  parent: mongoCluster
  name: 'allowAll'
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '255.255.255.255'
  }
}

resource openAiAccount 'Microsoft.CognitiveServices/accounts@2022-12-01' = {
  name: openAiSettings.name
  location: location
  sku: {
    name: openAiSettings.sku
  }
  kind: 'OpenAI'
  properties: {
    customSubDomainName: openAiSettings.name
    publicNetworkAccess: 'Enabled'
  }
}

resource openAiEmbeddingsModelDeployment 'Microsoft.CognitiveServices/accounts/deployments@2022-12-01' = {
  parent: openAiAccount
  name: openAiSettings.embeddingsModel.deployment.name
  properties: {
    model: {
      format: 'OpenAI'
      name: openAiSettings.embeddingsModel.name
      version: openAiSettings.embeddingsModel.version
    }
    scaleSettings: {
      scaleType: 'Standard'
    }
  }
}

resource openAiCompletionsModelDeployment 'Microsoft.CognitiveServices/accounts/deployments@2022-12-01' = {
  parent: openAiAccount
  name: openAiSettings.completionsModel.deployment.name
  properties: {
    model: {
      format: 'OpenAI'
      name: openAiSettings.completionsModel.name
      version: openAiSettings.completionsModel.version
    }
    scaleSettings: {
      scaleType: 'Standard'
    }
  }
  dependsOn:[
    openAiEmbeddingsModelDeployment
  ]
}

resource appServicePlan 'Microsoft.Web/serverfarms@2022-03-01' = {
  name: appServiceSettings.plan.name
  location: location
  sku: {
    name: appServiceSettings.plan.sku
  }
  kind: 'windows'
}

resource appServiceWeb 'Microsoft.Web/sites@2022-03-01' = {
  name: appServiceSettings.web.name
  location: location
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      netframeworkVersion: 'v8.0'
      webSocketsEnabled: true
      alwaysOn: true
    }
  }
}

resource appServiceApi 'Microsoft.Web/sites@2022-03-01' = {
  name: appServiceSettings.api.name
  location: location
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      netframeworkVersion: 'v8.0'
      webSocketsEnabled: true
      alwaysOn: true
    }
  }
}


resource appServiceWebSettings 'Microsoft.Web/sites/config@2022-03-01' = {
  parent: appServiceApi
  name: 'appsettings'
  kind: 'string'
  properties: {
    APPINSIGHTS_INSTRUMENTATIONKEY: appServiceWebInsights.properties.InstrumentationKey
    OPENAI__ENDPOINT: openAiAccount.properties.endpoint
    OPENAI__APIKEY: openAiAccount.listKeys().key1
    OPENAI__EMBEDDINGSDEPLOYMENTNAME: openAiEmbeddingsModelDeployment.name
    OPENAI__COMPLETIONSDEPLOYMENTNAME: openAiCompletionsModelDeployment.name
    OPENAI__MAXCONVERSATIONTOKENS: openAiSettings.maxConversationTokens
    OPENAI__MAXCOMPLETIONTOKENS: openAiSettings.maxCompletionTokens
    MONGODB__CONNECTION: 'mongodb+srv://${mongovCoreSettings.mongoClusterLogin}:${mongovCoreSettings.mongoClusterPassword}@${mongovCoreSettings.mongoClusterName}.mongocluster.cosmos.azure.com/?tls=true&authMechanism=SCRAM-SHA-256&retrywrites=false&maxIdleTimeMS=120000'
    MONGODB__DATABASENAME: 'retaildb'
    MONGODB__COLLECTIONNAMES: 'product,customer,vectors,completions'
    MONGODB__MAXVECTORSEARCHRESULTS: '10'
    APIBASEURL: 'https://${appServiceApi.properties.defaultHostName}/'
  }
}

resource appServiceApiSettings 'Microsoft.Web/sites/config@2022-03-01' = {
  parent: appServiceWeb
  name: 'appsettings'
  kind: 'string'
  properties: {
    APPINSIGHTS_INSTRUMENTATIONKEY: appServiceWebInsights.properties.InstrumentationKey
    OPENAI__ENDPOINT: openAiAccount.properties.endpoint
    OPENAI__APIKEY: openAiAccount.listKeys().key1
    OPENAI__EMBEDDINGSDEPLOYMENTNAME: openAiEmbeddingsModelDeployment.name
    OPENAI__COMPLETIONSDEPLOYMENTNAME: openAiCompletionsModelDeployment.name
    OPENAI__MAXCONVERSATIONTOKENS: openAiSettings.maxConversationTokens
    OPENAI__MAXCOMPLETIONTOKENS: openAiSettings.maxCompletionTokens
    MONGODB__CONNECTION: 'mongodb+srv://${mongovCoreSettings.mongoClusterLogin}:${mongovCoreSettings.mongoClusterPassword}@${mongovCoreSettings.mongoClusterName}.mongocluster.cosmos.azure.com/?tls=true&authMechanism=SCRAM-SHA-256&retrywrites=false&maxIdleTimeMS=120000'
    MONGODB__DATABASENAME: 'retaildb'
    MONGODB__COLLECTIONNAMES: 'product,customer,vectors,completions'
    MONGODB__MAXVECTORSEARCHRESULTS: '10'
    APIBASEURL: 'https://${appServiceApi.properties.defaultHostName}/'
  }
}


resource appServiceWebInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: appServiceWeb.name
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
  }
}

output deployedWebUrl string = appServiceSettings.web.name
output deployedApiUrl string = appServiceSettings.api.name
