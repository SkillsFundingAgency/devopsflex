<?xml version="1.0" encoding="utf-8"?>
<Dsl xmlns:dm0="http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core" dslVersion="1.0.0.0" Id="64492ec5-70ec-4c97-8ced-9e2d4bb018fe" Description="Description for DevOpsFlex.DSL.DevOpsFlex" Name="DevOpsFlex" DisplayName="DevOpsFlex" Namespace="DevOpsFlex.DSL" ProductName="DevOpsFlex" CompanyName="Skills Funding Agency" PackageGuid="9d96a236-19aa-425c-b3ac-027b81a83df6" PackageNamespace="DevOpsFlex.DSL" xmlns="http://schemas.microsoft.com/VisualStudio/2005/DslTools/DslDefinitionModel">
  <Classes>
    <DomainClass Id="0d4d9f14-7ca8-405e-88f7-59e9c36fab13" Description="Represents a DevOpsFlex System." Name="DevOpsSystem" DisplayName="DevOps System" Namespace="DevOpsFlex.DSL">
      <Properties>
        <DomainProperty Id="403c4358-9728-4d4f-8fb7-626f4b023b03" Description="Description for DevOpsFlex.DSL.DevOpsSystem.Name" Name="Name" DisplayName="Name" IsElementName="true">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
      </Properties>
      <ElementMergeDirectives>
        <ElementMergeDirective>
          <Index>
            <DomainClassMoniker Name="DevOpsAlert" />
          </Index>
          <LinkCreationPaths>
            <DomainPath>DevOpsSystemHasDevOpsAlerted.DevOpsAlerted</DomainPath>
          </LinkCreationPaths>
        </ElementMergeDirective>
      </ElementMergeDirectives>
    </DomainClass>
    <DomainClass Id="425a8abc-2f80-4791-a0df-f26b86731047" Description="Represents a DevOpsFlex Alert." Name="DevOpsAlert" DisplayName="DevOps Alert" HelpKeyword="fabrikam.minimalwpf.exampleElement" Namespace="DevOpsFlex.DSL">
      <Properties>
        <DomainProperty Id="d5245012-1be4-4888-8d61-c77573eabbc1" Description="Description for DevOpsFlex.DSL.DevOpsAlert.Name" Name="Name" DisplayName="Name" DefaultValue="Element" IsElementName="true">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="452e049e-1226-4d3f-b77d-a621084cb06f" Description="Description for DevOpsFlex.DSL.DevOpsAlert.Description" Name="Description" DisplayName="Description">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
        <DomainProperty Id="e6e60fd5-fbcb-4446-b5b4-0b56576f830e" Description="Description for DevOpsFlex.DSL.DevOpsAlert.Metric" Name="Metric" DisplayName="Metric">
          <Type>
            <ExternalTypeMoniker Name="/System/String" />
          </Type>
        </DomainProperty>
      </Properties>
    </DomainClass>
  </Classes>
  <Relationships>
    <DomainRelationship Id="988ea64a-5ec1-49e7-a2f4-7948abba5b7e" Description="Description for DevOpsFlex.DSL.DevOpsSystemHasDevOpsAlerted" Name="DevOpsSystemHasDevOpsAlerted" DisplayName="Dev Ops System Has Dev Ops Alerted" Namespace="DevOpsFlex.DSL" IsEmbedding="true">
      <Source>
        <DomainRole Id="909fe42f-7817-470b-a049-e142f33da930" Description="Description for DevOpsFlex.DSL.DevOpsSystemHasDevOpsAlerted.DevOpsSystem" Name="DevOpsSystem" DisplayName="Dev Ops System" PropertyName="DevOpsAlerted" PropagatesCopy="PropagatesCopyToLinkAndOppositeRolePlayer" PropertyDisplayName="Dev Ops Alerted">
          <RolePlayer>
            <DomainClassMoniker Name="DevOpsSystem" />
          </RolePlayer>
        </DomainRole>
      </Source>
      <Target>
        <DomainRole Id="93dbbcf6-e644-41bc-948e-0f50fff5b209" Description="Description for DevOpsFlex.DSL.DevOpsSystemHasDevOpsAlerted.DevOpsAlert" Name="DevOpsAlert" DisplayName="Dev Ops Alert" PropertyName="DevOpsSystem" Multiplicity="ZeroOne" PropagatesDelete="true" PropertyDisplayName="Dev Ops System">
          <RolePlayer>
            <DomainClassMoniker Name="DevOpsAlert" />
          </RolePlayer>
        </DomainRole>
      </Target>
    </DomainRelationship>
  </Relationships>
  <Types>
    <ExternalType Name="DateTime" Namespace="System" />
    <ExternalType Name="String" Namespace="System" />
    <ExternalType Name="Int16" Namespace="System" />
    <ExternalType Name="Int32" Namespace="System" />
    <ExternalType Name="Int64" Namespace="System" />
    <ExternalType Name="UInt16" Namespace="System" />
    <ExternalType Name="UInt32" Namespace="System" />
    <ExternalType Name="UInt64" Namespace="System" />
    <ExternalType Name="SByte" Namespace="System" />
    <ExternalType Name="Byte" Namespace="System" />
    <ExternalType Name="Double" Namespace="System" />
    <ExternalType Name="Single" Namespace="System" />
    <ExternalType Name="Guid" Namespace="System" />
    <ExternalType Name="Boolean" Namespace="System" />
    <ExternalType Name="Char" Namespace="System" />
  </Types>
  <XmlSerializationBehavior Name="DevOpsFlexSerializationBehavior" Namespace="DevOpsFlex.DSL">
    <ClassData>
      <XmlClassData TypeName="DevOpsSystem" MonikerAttributeName="name" SerializeId="true" MonikerElementName="devOpsSystemMoniker" ElementName="devOpsSystem" MonikerTypeName="DevOpsSystemMoniker">
        <DomainClassMoniker Name="DevOpsSystem" />
        <ElementData>
          <XmlRelationshipData RoleElementName="devOpsAlerted">
            <DomainRelationshipMoniker Name="DevOpsSystemHasDevOpsAlerted" />
          </XmlRelationshipData>
          <XmlPropertyData XmlName="name" IsMonikerKey="true">
            <DomainPropertyMoniker Name="DevOpsSystem/Name" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="DevOpsAlert" MonikerAttributeName="Name" SerializeId="true" MonikerElementName="devOpsAlertMoniker" ElementName="devOpsAlert" MonikerTypeName="DevOpsAlertMoniker">
        <DomainClassMoniker Name="DevOpsAlert" />
        <ElementData>
          <XmlPropertyData XmlName="Name" IsMonikerKey="true">
            <DomainPropertyMoniker Name="DevOpsAlert/Name" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="description">
            <DomainPropertyMoniker Name="DevOpsAlert/Description" />
          </XmlPropertyData>
          <XmlPropertyData XmlName="metric">
            <DomainPropertyMoniker Name="DevOpsAlert/Metric" />
          </XmlPropertyData>
        </ElementData>
      </XmlClassData>
      <XmlClassData TypeName="DevOpsSystemHasDevOpsAlerted" MonikerAttributeName="" SerializeId="true" MonikerElementName="devOpsSystemHasDevOpsAlertedMoniker" ElementName="devOpsSystemHasDevOpsAlerted" MonikerTypeName="DevOpsSystemHasDevOpsAlertedMoniker">
        <DomainRelationshipMoniker Name="DevOpsSystemHasDevOpsAlerted" />
      </XmlClassData>
    </ClassData>
  </XmlSerializationBehavior>
  <ExplorerBehavior Name="DevOpsFlexExplorerBehavior" />
  <CustomEditor CopyPasteGeneration="CopyPasteOnly" FileExtension="flex" EditorGuid="9cc103fc-44f8-46b8-b35d-44ea5f7e51dc">
    <RootClass>
      <DomainClassMoniker Name="DevOpsSystem" />
    </RootClass>
    <XmlSerializationDefinition CustomPostLoad="false">
      <XmlSerializationBehaviorMoniker Name="DevOpsFlexSerializationBehavior" />
    </XmlSerializationDefinition>
    <Validation UsesMenu="false" UsesOpen="false" UsesSave="false" UsesLoad="false" />
  </CustomEditor>
  <Explorer ExplorerGuid="447efe86-60c9-423e-8815-7c596db3b46e" Title="WpfDesigner Explorer">
    <ExplorerBehaviorMoniker Name="DevOpsFlex/DevOpsFlexExplorerBehavior" />
  </Explorer>
</Dsl>