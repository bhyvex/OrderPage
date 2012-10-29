<?xml version="1.0" encoding="utf-8"?>
<configurationSectionModel xmlns:dm0="http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core" dslVersion="1.0.0.0" Id="46147fb7-ba4e-410f-91eb-7f9c681dc0fe" namespace="Atomia.Web.Plugin.PublicOrder.Configurations" xmlSchemaNamespace="Atomia.Web.Plugin.PublicOrder.Configurations" xmlns="http://schemas.microsoft.com/dsltools/ConfigurationSectionDesigner">
  <typeDefinitions>
    <externalType name="String" namespace="System" />
    <externalType name="Boolean" namespace="System" />
    <externalType name="Int32" namespace="System" />
    <externalType name="Int64" namespace="System" />
    <externalType name="Single" namespace="System" />
    <externalType name="Double" namespace="System" />
    <externalType name="DateTime" namespace="System" />
    <externalType name="TimeSpan" namespace="System" />
  </typeDefinitions>
  <configurationElements>
    <configurationSection name="PublicOrderConfigurationSection" namespace="Atomia.Web.Plugin.PublicOrder.Configurations" codeGenOptions="Singleton, XmlnsProperty" xmlSectionName="publicOrderConfigurationSection">
      <elementProperties>
        <elementProperty name="DefaultCountry" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="defaultCountry" isReadOnly="false">
          <type>
            <configurationElementMoniker name="/46147fb7-ba4e-410f-91eb-7f9c681dc0fe/DefaultCountry" />
          </type>
        </elementProperty>
        <elementProperty name="OnlinePayment" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="onlinePayment" isReadOnly="false">
          <type>
            <configurationElementMoniker name="/46147fb7-ba4e-410f-91eb-7f9c681dc0fe/OnlinePayment" />
          </type>
        </elementProperty>
        <elementProperty name="InvoiceByEmail" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="invoiceByEmail" isReadOnly="false">
          <type>
            <configurationElementMoniker name="/46147fb7-ba4e-410f-91eb-7f9c681dc0fe/InvoiceByEmail" />
          </type>
        </elementProperty>
        <elementProperty name="InvoiceByPost" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="invoiceByPost" isReadOnly="false">
          <type>
            <configurationElementMoniker name="/46147fb7-ba4e-410f-91eb-7f9c681dc0fe/InvoiceByPost" />
          </type>
        </elementProperty>
        <elementProperty name="QueryStringList" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="queryStringList" isReadOnly="false">
          <type>
            <configurationElementCollectionMoniker name="/46147fb7-ba4e-410f-91eb-7f9c681dc0fe/QueryStringList" />
          </type>
        </elementProperty>
        <elementProperty name="CountriesList" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="countriesList" isReadOnly="false">
          <type>
            <configurationElementCollectionMoniker name="/46147fb7-ba4e-410f-91eb-7f9c681dc0fe/CountriesList" />
          </type>
        </elementProperty>
        <elementProperty name="PayPal" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="payPal" isReadOnly="false">
          <type>
            <configurationElementMoniker name="/46147fb7-ba4e-410f-91eb-7f9c681dc0fe/PayPal" />
          </type>
        </elementProperty>
        <elementProperty name="PayexRedirect" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="payexRedirect" isReadOnly="false">
          <type>
            <configurationElementMoniker name="/46147fb7-ba4e-410f-91eb-7f9c681dc0fe/PayexRedirect" />
          </type>
        </elementProperty>
        <elementProperty name="WorldPay" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="worldPay" isReadOnly="false">
          <type>
            <configurationElementMoniker name="/46147fb7-ba4e-410f-91eb-7f9c681dc0fe/WorldPay" />
          </type>
        </elementProperty>
        <elementProperty name="DibsFlexwin" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="dibsFlexwin" isReadOnly="false">
          <type>
            <configurationElementMoniker name="/46147fb7-ba4e-410f-91eb-7f9c681dc0fe/DibsFlexwin" />
          </type>
        </elementProperty>
      </elementProperties>
    </configurationSection>
    <configurationElementCollection name="QueryStringList" namespace="Atomia.Web.Plugin.PublicOrder.Configurations" xmlItemName="queryString" codeGenOptions="Indexer, AddMethod, RemoveMethod">
      <itemType>
        <configurationElementMoniker name="/46147fb7-ba4e-410f-91eb-7f9c681dc0fe/QueryString" />
      </itemType>
    </configurationElementCollection>
    <configurationElement name="QueryString" namespace="Atomia.Web.Plugin.PublicOrder.Configurations">
      <attributeProperties>
        <attributeProperty name="Name" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="name" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/46147fb7-ba4e-410f-91eb-7f9c681dc0fe/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="PassToView" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="passToView" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/46147fb7-ba4e-410f-91eb-7f9c681dc0fe/Boolean" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElement name="DefaultCountry" namespace="Atomia.Web.Plugin.PublicOrder.Configurations">
      <attributeProperties>
        <attributeProperty name="Name" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="name" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/46147fb7-ba4e-410f-91eb-7f9c681dc0fe/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="Code" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="code" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/46147fb7-ba4e-410f-91eb-7f9c681dc0fe/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElement name="OnlinePayment" namespace="Atomia.Web.Plugin.PublicOrder.Configurations">
      <attributeProperties>
        <attributeProperty name="Enabled" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="enabled" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/46147fb7-ba4e-410f-91eb-7f9c681dc0fe/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="Default" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="default" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/46147fb7-ba4e-410f-91eb-7f9c681dc0fe/Boolean" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElement name="InvoiceByEmail">
      <attributeProperties>
        <attributeProperty name="Enabled" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="enabled" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/46147fb7-ba4e-410f-91eb-7f9c681dc0fe/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="Default" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="default" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/46147fb7-ba4e-410f-91eb-7f9c681dc0fe/Boolean" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElement name="InvoiceByPost">
      <attributeProperties>
        <attributeProperty name="Enabled" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="enabled" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/46147fb7-ba4e-410f-91eb-7f9c681dc0fe/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="Default" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="default" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/46147fb7-ba4e-410f-91eb-7f9c681dc0fe/Boolean" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElementCollection name="CountriesList" namespace="Atomia.Web.Plugin.PublicOrder.Configurations" xmlItemName="countryItem" codeGenOptions="Indexer, AddMethod, RemoveMethod, GetItemMethods">
      <itemType>
        <configurationElementMoniker name="/46147fb7-ba4e-410f-91eb-7f9c681dc0fe/CountryItem" />
      </itemType>
    </configurationElementCollection>
    <configurationElement name="CountryItem" namespace="Atomia.Web.Plugin.PublicOrder.Configurations">
      <attributeProperties>
        <attributeProperty name="Name" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="name" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/46147fb7-ba4e-410f-91eb-7f9c681dc0fe/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="Code" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="code" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/46147fb7-ba4e-410f-91eb-7f9c681dc0fe/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="Default" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="default" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/46147fb7-ba4e-410f-91eb-7f9c681dc0fe/Boolean" />
          </type>
        </attributeProperty>
        <attributeProperty name="Image" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="image" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/46147fb7-ba4e-410f-91eb-7f9c681dc0fe/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="Currency" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="currency" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/46147fb7-ba4e-410f-91eb-7f9c681dc0fe/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElement name="PayPal">
      <attributeProperties>
        <attributeProperty name="Enabled" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="enabled" isReadOnly="true">
          <type>
            <externalTypeMoniker name="/46147fb7-ba4e-410f-91eb-7f9c681dc0fe/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="Default" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="default" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/46147fb7-ba4e-410f-91eb-7f9c681dc0fe/Boolean" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElement name="PayexRedirect">
      <attributeProperties>
        <attributeProperty name="Enabled" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="enabled" isReadOnly="true">
          <type>
            <externalTypeMoniker name="/46147fb7-ba4e-410f-91eb-7f9c681dc0fe/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="Default" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="default" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/46147fb7-ba4e-410f-91eb-7f9c681dc0fe/Boolean" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElement name="WorldPay">
      <attributeProperties>
        <attributeProperty name="Enabled" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="enabled" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/46147fb7-ba4e-410f-91eb-7f9c681dc0fe/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="Default" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="default" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/46147fb7-ba4e-410f-91eb-7f9c681dc0fe/Boolean" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElement name="DibsFlexwin">
      <attributeProperties>
        <attributeProperty name="Enabled" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="enabled" isReadOnly="true">
          <type>
            <externalTypeMoniker name="/46147fb7-ba4e-410f-91eb-7f9c681dc0fe/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="Default" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="default" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/46147fb7-ba4e-410f-91eb-7f9c681dc0fe/Boolean" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
  </configurationElements>
  <propertyValidators>
    <validators />
  </propertyValidators>
</configurationSectionModel>