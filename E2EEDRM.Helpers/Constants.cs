using kCura.Relativity.Client;
using Relativity.Imaging.Services.Interfaces;
using Relativity.Processing.Services;
using Relativity.Productions.Services;
using System;
using System.Collections.Generic;

namespace E2EEDRM.Helpers
{
	public class Constants
	{
		public static readonly bool DebugMode = false;

		public class Instance
		{
			public const string TENANT_NAME = "demo-instance";
			public const string BASE_INSTANCE_URL = "https://" + TENANT_NAME + ".relativity.one";
			public const string BASE_RELATIVITY_URL_NO_SERVICES = "https://" + TENANT_NAME + ".relativity.one/Relativity";
			public const string BASE_RELATIVITY_URL = "https://" + TENANT_NAME + "-services.relativity.one/Relativity";
			public const string RELATIVITY_ADMIN_USER_NAME = "relativity_admin_user_name";
			public const string RELATIVITY_ADMIN_PASSWORD = "relativity_admin_password";
		}

		public class Guids
		{
			public class ObjectType
			{
				public static Guid RelativityTimeZone = new Guid("BC411C4D-285A-466B-8824-E084FD981F8B");
			}

			public class Fields
			{
				public class ProcessingSet
				{
					public static Guid InventoryStatus = new Guid("D04EF741-C622-4993-9A53-2F2E54F4D1D7");
					public static Guid DiscoverStatus = new Guid("513DD373-661B-4EA5-9AC4-43BEA2F793EE");
					public static Guid PublishStatus = new Guid("E3343C3E-0FFA-4846-B4D3-CB1E5A37140C");
				}

				public class ImagingSet
				{
					public static Guid Status = new Guid("030747E3-E154-4DF1-BD10-CF6C9734D10A");
				}

				public class ProductionSet
				{
					public static Guid Status = new Guid("9367D894-2C3D-42B1-91D0-B6931E1A62D4");
				}
			}
		}

		public enum ProcessingJobType
		{
			Inventory,
			Discover,
			Publish
		}

		public const int DOCUMENT_ARTIFACT_TYPE = 10;

		public class Waiting
		{
			public const int MAX_WAIT_TIME_IN_MINUTES = 10;
			public const int SLEEP_TIME_IN_SECONDS = 15;
		}

		public class Imaging
		{
			public class Profile
			{
				public const string NAME = "E2E Imaging Profile";
				public const int IMAGE_OUTPUT_DPI = 100;
				public const ImageFormat BASIC_IMAGE_FORMAT = ImageFormat.Jpeg;
				public const string BASIC_IMAGE_FORMAT_AS_STRING = "Jpeg";
				public const ImageSize IMAGE_SIZE = ImageSize.A4;
				public const string IMAGE_SIZE_AS_STRING = "A4";
				public const ImagingMethod IMAGING_METHOD = ImagingMethod.Basic;
				public const string IMAGING_METHOD_AS_STRING = "Basic";
			}

			public class Set
			{
				public const string NAME = "E2E Imaging Set";
				public const string EMAIL_NOTIFICATION_RECIPIENTS = "";
			}

			public class Job
			{
				public const bool QC_ENABLED = false;
			}
		}

		public class Processing
		{
			public class Profile
			{
				public const string NAME = "E2E Processing Profile";
				public const string DEFAULT_DOCUMENT_NUMBERING_PREFIX = "abc";
				public const NumberOfDigits NUMBER_OF_DIGITS4 = NumberOfDigits.NumberOfDigits4;
				public const string NUMBER_OF_DIGITS4_AS_STRING = "NumberOfDigits4";
				public const Relativity.Processing.Services.NumberingType NUMBERING_TYPE = Relativity.Processing.Services.NumberingType.AutoNumber;
				public const string NUMBERING_TYPE_AS_STRING = "AutoNumber";
				public const ParentChildNumbering PARENT_CHILD_NUMBERING = ParentChildNumbering.ContinuousAlways;
				public const string PARENT_CHILD_NUMBERING_AS_STRING = "ContinuousAlways";
				public const string DELIMITER = "Period";
				public const DeduplicationMethod DE_DUPLICATION_METHOD = DeduplicationMethod.None;
				public const string DE_DUPLICATION_METHOD_AS_STRING = "None";
				public const bool PROPAGATE_DE_DUPLICATION_DATA = false;
				public const bool EXTRACT_CHILDREN = false;
				public const EmailOutput EMAIL_OUTPUT = EmailOutput.MSG;
				public const string EMAIL_OUTPUT_AS_STRING = "MSG";
				public const ExcelTextExtractionMethod EXCEL_TEXT_EXTRACTION_METHOD = ExcelTextExtractionMethod.Native;
				public const string EXCEL_TEXT_EXTRACTION_METHOD_AS_STRING = "Native";
				public const ExcelHeaderFooterExtraction EXCEL_HEADER_FOOTER_EXTRACTION = ExcelHeaderFooterExtraction.DoNotExtract;
				public const string EXCEL_HEADER_FOOTER_EXTRACTION_AS_STRING = "DoNotExtract";
				public const PowerPointTextExtractionMethod POWER_POINT_TEXT_EXTRACTION_METHOD = PowerPointTextExtractionMethod.Native;
				public const string POWER_POINT_TEXT_EXTRACTION_METHOD_AS_STRING = "Native";
				public const WordTextExtractionMethod WORD_TEXT_EXTRACTION_METHOD = WordTextExtractionMethod.Native;
				public const string WORD_TEXT_EXTRACTION_METHOD_AS_STRING = "Native";
				public const bool OCR = true;
				public const OCRAccuracy OCR_ACCURACY = OCRAccuracy.Low;
				public const string OCR_ACCURACY_AS_STRING = "Low";
				public const bool OCR_TEXT_SEPARATOR = true;
				public const bool DE_NIST = false;
				public static readonly HashSet<OcrLanguage> DefaultOcrLanguages = new HashSet<OcrLanguage> { OcrLanguage.English };
				public static readonly string[] DEFAULT_OCR_LANGUAGES = new[] { "English" };
				public const bool AUTO_PUBLISH_SET = false;
				public const bool USE_SOURCE_FOLDER_STRUCTURE = true;
			}

			public class Custodian
			{
				public const int ARTIFACT_ID = 0; // Indicates a new ProcessingCustodian object
				public const string DOCUMENT_NUMBERING_PREFIX = "REL";
				public const string FIRST_NAME = "John";
				public const string LAST_NAME = "Smith";
			}

			public class Set
			{
				public const int ARTIFACT_ID = 0; // Indicates a new ProcessingSet object
				public static readonly string[] EmailNotificationRecipients = new string[] { }; //Example: new string[] { "johnSmith@domain.com", "adamJohnson@domain.com" }
				public const string NAME = "E2E Processing Set";
			}

			public class DataSource
			{
				public const int ARTIFACT_ID = 0; // Indicates a new ProcessingDataSource object
				public const string DOCUMENT_NUMBERING_PREFIX = "abc";
				public const string INPUT_PATH = @"\\files\T002\ProcessingSource\E2E documents\TransferApiDocuments";
				public const string NAME = "E2E Data Source";
				public static readonly OcrLanguage[] OcrLanguages = new[] { OcrLanguage.English };
				public const int ORDER = 200;
				public const int START_NUMBER = 8;
				public const bool IS_START_NUMBER_VISIBLE = true;
			}
		}

		public class Production
		{
			public const string NAME = "E2E Production";
			public static readonly DateTime DateProduced = DateTime.Now;
			public const string EMAIL_RECIPIENTS = "joe.hendersonstaudt@relativity.com";
			public const bool SCALE_BRANDING_FONT = true;
			public const int BRANDING_FONT_SIZE = 25;
			public const PlaceholderImageFormat PLACEHOLDER_IMAGE_FORMAT = PlaceholderImageFormat.Tiff;
			public const string BATES_PREFIX = "E2E";
			public const string BATES_SUFFIX = "";
			public const int BATES_START_NUMBER = 0001;
			public const int NUMBER_OF_DIGITS_FOR_DOCUMENT_NUMBERING = 5;
			public const string HEADER_FOOTER_FRIENDLY_NAME = "Left Footer";
			public const HeaderFooterType HEADER_FOOTER_TYPE = HeaderFooterType.FreeText;
			public const string HEADER_FOOTER_FREE_TEXT = "Produced via End2End solution";
			public const string KEYWORDS = "PageLevel, Complete Setting";

			public class DataSource
			{
				public static string Name { get; } = "Responsive Documents";
				public const ProductionType PRODUCTION_TYPE = ProductionType.ImagesAndNatives;
				public const string PRODUCTION_TYPE_AS_STRING = "ImagesAndNatives";
				public const UseImagePlaceholderOption USE_IMAGE_PLACEHOLDER = UseImagePlaceholderOption.NeverUseImagePlaceholder;
				public const string USE_IMAGE_PLACEHOLDER_AS_STRING = "NeverUseImagePlaceholder";
				public const bool BURN_REDACTIONS = false;
			}
		}

		public class Search
		{
			public class KeywordSearch
			{
				public const string OWNER = "Public";
				public const string NAME = "Extracted Text";
				public const string FIELD_EDIT = "Edit";
				public const string FIELD_FILE_ICON = "File Icon";
				public const string FIELD_CONTROL_NUMBER = "Control Number";
				public const string CONDITION_FIELD_EXTRACTED_TEXT = "Extracted Text";
				public const string NOTES = "Search for dtIndex seeding";
			}
			public class DtSearch
			{
				public const string NAME = "End to End dtSearch for Production";
				public const string NAME_EXTRACTED_TEXT = "End to End dtSearch for Extracted Text";
				public const string OWNER = "Public";
				public const string FIELD_EDIT = "Edit";
				public const string FIELD_FILE_ICON = "File Icon";
				public const string FIELD_CONTROL_NUMBER = "Control Number";
				public const string NOTES = "End2End dtSearch for Production";
			}

			public class DtSearchIndex
			{
				public const string NAME = "E2E dtSearch Index";
				public const int ORDER = 10;
				public const bool RECOGNIZE_DATES = true;
				public const bool SKIP_NUMERIC_VALUES = true;
				public const string EMAIL_ADDRESS = "";
				public const string NOISE_WORDS = "";
				public const string ALPHABET_TEXT = "";
				public const string DIRTY_SETTINGS = "";
				public const int SUB_INDEX_SIZE = 250000;
				public const int FRAGMENTATION_THRESHOLD = 9;
				public const int PRIORITY = 9;
			}
		}

		public class Workspace
		{
			public const string NAME = "E2E Test";
			public const bool ACCESSIBLE = true;
			public const string DATABASE_LOCATION = @"ST002EDDS.T002.ctus010000.relativity.one,24331"; //"ST002EDDS.T002.ctus015128.relativity.one,24331";// @"ctus01512802W01.sql-y009.relativity.one\ctus01512802W01";
			public const string EXTRACTED_TEXT_FIELD_NAME = "Extracted Text";
			public const string WORKSPACE_TEMPLATE_NAME = "RelativityOne Quick Start Template";

			public class ResponsiveField
			{
				public static string Name { get; } = "Responsive - E2E";
				public const bool VALUE = true;
				public const FieldType FIELD_TYPE_ID = FieldType.YesNo;
				public const bool IS_REQUIRED = false;
				public const bool OPEN_TO_ASSOCIATIONS = false;
				public const bool LINKED = false;
				public const bool ALLOW_SORT_TALLY = true;
				public const bool WRAPPING = true;
				public const bool ALLOW_GROUP_BY = false;
				public const bool ALLOW_PIVOT = false;
				public const bool IGNORE_WARNINGS = true;
				public const string WIDTH = "";
				public const string NO_VALUE = "No";
				public const string YES_VALUE = "Yes";
			}
		}

		public class Transfer
		{
			public const string LOCAL_DOCUMENTS_FOLDER_PATH = "ProcessingDocuments";
			public const string REMOTE_DOCUMENTS_FOLDER_PATH = "/ProcessingSource/E2E documents/TransferApiDocuments";
		}

		public class BlobFuse
		{
			public class AzureSecrets
			{
				public const string AZURE_STORAGE_ACCOUNT_NAME = "AZURE_STORAGE_ACCOUNT_NAME";
				public const string AZURE_STORAGE_ACCOUNT_KEY = "AZURE_STORAGE_ACCOUNT_KEY";
				public const string AZURE_STORAGE_CONNECTION_STRING = "DefaultEndpointsProtocol=https;AccountName=" + AZURE_STORAGE_ACCOUNT_NAME + ";AccountKey=" + AZURE_STORAGE_ACCOUNT_KEY;
				public const string CONTAINER_NAME = @"CONTAINER_NAME";
				public const string AZURE_STORAGE_BLOB_CONTAINER_SAS_KEY = "AZURE_STORAGE_BLOB_CONTAINER_SAS_KEY";
				public const string AZURE_STORAGE_BLOB_CONTAINER_SAS_URI = "https://" + AZURE_STORAGE_ACCOUNT_NAME + ".blob.core.windows.net/?" + AZURE_STORAGE_BLOB_CONTAINER_SAS_KEY;
			}

			public const string REMOTE_DOCUMENTS_FOLDER_PATH = "E2E documents/TransferApiDocuments";
		}

		public class ProductionExport
		{
			public const string RdcExecutablePath = @"C:\Program Files\kCura Corporation\Relativity Desktop Client\kCura.EDDS.WinForm.exe";
			public const string ExportedProductionFolderName = "ExportedProduction";
			public const string ExportSettingsFileName = "ExportSettings.kwx";
			public static string ExportSettingsTemplate =
				@"<SOAP-ENV:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:SOAP-ENC=""http://schemas.xmlsoap.org/soap/encoding/"" xmlns:SOAP-ENV=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:clr=""http://schemas.microsoft.com/soap/encoding/clr/1.0"" SOAP-ENV:encodingStyle=""http://schemas.xmlsoap.org/soap/encoding/"">
				<SOAP-ENV:Body>
				<a1:ExtendedExportFile id=""ref-1"" xmlns:a1=""http://schemas.microsoft.com/clr/nsassem/kCura.WinEDDS/kCura.WinEDDS%2C%20Version%3D10.2.99.73%2C%20Culture%3Dneutral%2C%20PublicKeyToken%3Dnull"">
				<SelectedNativesNameViewFields href=""#ref-3""/>
				<ArtifactID>*ProductionArtifactId*</ArtifactID>
				<LoadFilesPrefix id=""ref-4"">End2End Production</LoadFilesPrefix>
				<NestedValueDelimiter>92</NestedValueDelimiter>
				<TypeOfExport>0</TypeOfExport>
				<FolderPath id=""ref-5"">*ExportFolderPath*</FolderPath>
				<ViewID>0</ViewID>
				<Overwrite>true</Overwrite>
				<RecordDelimiter>20</RecordDelimiter>
				<QuoteDelimiter>254</QuoteDelimiter>
				<NewlineDelimiter>174</NewlineDelimiter>
				<MultiRecordDelimiter>59</MultiRecordDelimiter>
				<ExportFullText>false</ExportFullText>
				<ExportFullTextAsFile>false</ExportFullTextAsFile>
				<ExportNative>false</ExportNative>
				<LogFileFormat id=""ref-6"">0</LogFileFormat>
				<RenameFilesToIdentifier>true</RenameFilesToIdentifier>
				<IdentifierColumnName id=""ref-7"">Control Number</IdentifierColumnName>
				<LoadFileExtension id=""ref-8"">html</LoadFileExtension>
				<ExportImages>true</ExportImages>
				<ExportNativesToFileNamedFrom>1</ExportNativesToFileNamedFrom>
				<FilePrefix id=""ref-9""></FilePrefix>
				<TypeOfExportedFilePath>0</TypeOfExportedFilePath>
				<TypeOfImage id=""ref-10"">2</TypeOfImage>
				<AppendOriginalFileName>false</AppendOriginalFileName>
				<LoadFileIsHtml>true</LoadFileIsHtml>
				<MulticodesAsNested>false</MulticodesAsNested>
				<LoadFileEncoding>65001</LoadFileEncoding>
				<TextFileEncoding>-1</TextFileEncoding>
				<VolumeDigitPadding>2</VolumeDigitPadding>
				<SubdirectoryDigitPadding>3</SubdirectoryDigitPadding>
				<StartAtDocumentNumber>0</StartAtDocumentNumber>
				<VolumeInfo href=""#ref-11""/>
				<SelectedTextFields xsi:null=""1""/>
				<ImagePrecedence href=""#ref-12""/>
				<SelectedViewFields href=""#ref-13""/>
				<ObjectTypeName id=""ref-14"">Document</ObjectTypeName>
				<UseCustomFileNaming>true</UseCustomFileNaming>
				<CustomFileNaming href=""#ref-15""/>
				</a1:ExtendedExportFile>
				<SOAP-ENC:Array id=""ref-3"" SOAP-ENC:arrayType=""a1:ViewFieldInfo[1]"" xmlns:a1=""http://schemas.microsoft.com/clr/nsassem/kCura.WinEDDS/kCura.WinEDDS%2C%20Version%3D10.2.99.73%2C%20Culture%3Dneutral%2C%20PublicKeyToken%3Dnull"">
				<item href=""#ref-16""/>
				</SOAP-ENC:Array>
				<a2:VolumeInfo id=""ref-11"" xmlns:a2=""http://schemas.microsoft.com/clr/nsassem/kCura.WinEDDS.Exporters/kCura.WinEDDS%2C%20Version%3D10.2.99.73%2C%20Culture%3Dneutral%2C%20PublicKeyToken%3Dnull"">
				<CopyNativeFilesFromRepository>true</CopyNativeFilesFromRepository>
				<CopyImageFilesFromRepository>true</CopyImageFilesFromRepository>
				<SubdirectoryMaxSize>500</SubdirectoryMaxSize>
				<SubdirectoryStartNumber>1</SubdirectoryStartNumber>
				<SubdirectoryFullTextPrefix id=""ref-17"">TEXT</SubdirectoryFullTextPrefix>
				<SubdirectoryNativePrefix id=""ref-18"">NATIVE</SubdirectoryNativePrefix>
				<SubdirectoryImagePrefix id=""ref-19"">IMG</SubdirectoryImagePrefix>
				<VolumeMaxSize>650</VolumeMaxSize>
				<VolumeStartNumber>1</VolumeStartNumber>
				<VolumePrefix id=""ref-20"">VOL</VolumePrefix>
				</a2:VolumeInfo>
				<SOAP-ENC:Array id=""ref-12"" SOAP-ENC:arrayType=""a1:Pair[1]"" xmlns:a1=""http://schemas.microsoft.com/clr/nsassem/kCura.WinEDDS/kCura.WinEDDS%2C%20Version%3D10.2.99.73%2C%20Culture%3Dneutral%2C%20PublicKeyToken%3Dnull"">
				<item href=""#ref-21""/>
				</SOAP-ENC:Array>
				<SOAP-ENC:Array id=""ref-13"" SOAP-ENC:arrayType=""a1:ViewFieldInfo[2]"" xmlns:a1=""http://schemas.microsoft.com/clr/nsassem/kCura.WinEDDS/kCura.WinEDDS%2C%20Version%3D10.2.99.73%2C%20Culture%3Dneutral%2C%20PublicKeyToken%3Dnull"">
				<item href=""#ref-22""/>
				<item href=""#ref-23""/>
				</SOAP-ENC:Array>
				<a3:CustomFileNameDescriptorModel id=""ref-15"" xmlns:a3=""http://schemas.microsoft.com/clr/nsassem/FileNaming.CustomFileNaming/kCura.WinEDDS%2C%20Version%3D10.2.99.73%2C%20Culture%3Dneutral%2C%20PublicKeyToken%3Dnull"">
				<_firstField href=""#ref-24""/>
				<_extendedDescriptors href=""#ref-25""/>
				</a3:CustomFileNameDescriptorModel>
				<a1:ViewFieldInfo id=""ref-16"" xmlns:a1=""http://schemas.microsoft.com/clr/nsassem/kCura.WinEDDS/kCura.WinEDDS%2C%20Version%3D10.2.99.73%2C%20Culture%3Dneutral%2C%20PublicKeyToken%3Dnull"">
				<_fieldArtifactId>1003667</_fieldArtifactId>
				<_avfId>1000186</_avfId>
				<_category>Identifier</_category>
				<_connectorFieldCategory>-1</_connectorFieldCategory>
				<_displayName id=""ref-27"">Control Number</_displayName>
				<_avfColumnName id=""ref-28"">ControlNumber</_avfColumnName>
				<_avfHeaderName id=""ref-29"">Control Number</_avfHeaderName>
				<_allowFieldName href=""#ref-9""/>
				<_columnSource>MainTable</_columnSource>
				<_dataSource id=""ref-30"">Document</_dataSource>
				<_sourceFieldName id=""ref-31"">Control Number</_sourceFieldName>
				<_sourceFieldArtifactTypeID>-1</_sourceFieldArtifactTypeID>
				<_sourceFieldArtifactID>-1</_sourceFieldArtifactID>
				<_connectorFieldArtifactID>-1</_connectorFieldArtifactID>
				<_sourceFieldArtifactTypeTableName href=""#ref-9""/>
				<_connectorFieldName href=""#ref-9""/>
				<_fieldType>Varchar</_fieldType>
				<_isLinked>true</_isLinked>
				<_fieldCodeTypeID>-1</_fieldCodeTypeID>
				<_artifactTypeID>10</_artifactTypeID>
				<_artifactTypeTableName id=""ref-32"">Document</_artifactTypeTableName>
				<_fieldIsArtifactBaseField>false</_fieldIsArtifactBaseField>
				<_formatString href=""#ref-9""/>
				<_isUnicodeEnabled>true</_isUnicodeEnabled>
				<_allowHtml>false</_allowHtml>
				<_parentFileFieldArtifactID>-1</_parentFileFieldArtifactID>
				<_parentFileFieldDisplayName href=""#ref-9""/>
				<_associativeArtifactTypeID>-1</_associativeArtifactTypeID>
				<_relationalTableName href=""#ref-9""/>
				<_relationalTableColumnName href=""#ref-9""/>
				<_relationalTableColumnName2 href=""#ref-9""/>
				<_parentReflectionType>Empty</_parentReflectionType>
				<_reflectedFieldArtifactTypeTableName href=""#ref-9""/>
				<_reflectedFieldIdentifierColumnName href=""#ref-9""/>
				<_reflectedFieldConnectorFieldName href=""#ref-9""/>
				<_reflectedConnectorIdentifierColumnName href=""#ref-9""/>
				<_enableDataGrid>false</_enableDataGrid>
				<_isVirtualAssociativeArtifactType>false</_isVirtualAssociativeArtifactType>
				<ViewFieldInfo_x002B__fieldArtifactId>1003667</ViewFieldInfo_x002B__fieldArtifactId>
				<ViewFieldInfo_x002B__avfId>1000186</ViewFieldInfo_x002B__avfId>
				<ViewFieldInfo_x002B__category>Identifier</ViewFieldInfo_x002B__category>
				<ViewFieldInfo_x002B__connectorFieldCategory>-1</ViewFieldInfo_x002B__connectorFieldCategory>
				<ViewFieldInfo_x002B__displayName href=""#ref-27""/>
				<ViewFieldInfo_x002B__avfColumnName href=""#ref-28""/>
				<ViewFieldInfo_x002B__avfHeaderName href=""#ref-29""/>
				<ViewFieldInfo_x002B__allowFieldName href=""#ref-9""/>
				<ViewFieldInfo_x002B__columnSource>MainTable</ViewFieldInfo_x002B__columnSource>
				<ViewFieldInfo_x002B__dataSource href=""#ref-30""/>
				<ViewFieldInfo_x002B__sourceFieldName href=""#ref-31""/>
				<ViewFieldInfo_x002B__sourceFieldArtifactTypeID>-1</ViewFieldInfo_x002B__sourceFieldArtifactTypeID>
				<ViewFieldInfo_x002B__sourceFieldArtifactID>-1</ViewFieldInfo_x002B__sourceFieldArtifactID>
				<ViewFieldInfo_x002B__connectorFieldArtifactID>-1</ViewFieldInfo_x002B__connectorFieldArtifactID>
				<ViewFieldInfo_x002B__sourceFieldArtifactTypeTableName href=""#ref-9""/>
				<ViewFieldInfo_x002B__connectorFieldName href=""#ref-9""/>
				<ViewFieldInfo_x002B__fieldType>Varchar</ViewFieldInfo_x002B__fieldType>
				<ViewFieldInfo_x002B__isLinked>true</ViewFieldInfo_x002B__isLinked>
				<ViewFieldInfo_x002B__fieldCodeTypeID>-1</ViewFieldInfo_x002B__fieldCodeTypeID>
				<ViewFieldInfo_x002B__artifactTypeID>10</ViewFieldInfo_x002B__artifactTypeID>
				<ViewFieldInfo_x002B__artifactTypeTableName href=""#ref-32""/>
				<ViewFieldInfo_x002B__fieldIsArtifactBaseField>false</ViewFieldInfo_x002B__fieldIsArtifactBaseField>
				<ViewFieldInfo_x002B__formatString href=""#ref-9""/>
				<ViewFieldInfo_x002B__isUnicodeEnabled>true</ViewFieldInfo_x002B__isUnicodeEnabled>
				<ViewFieldInfo_x002B__allowHtml>false</ViewFieldInfo_x002B__allowHtml>
				<ViewFieldInfo_x002B__parentFileFieldArtifactID>-1</ViewFieldInfo_x002B__parentFileFieldArtifactID>
				<ViewFieldInfo_x002B__parentFileFieldDisplayName href=""#ref-9""/>
				<ViewFieldInfo_x002B__associativeArtifactTypeID>-1</ViewFieldInfo_x002B__associativeArtifactTypeID>
				<ViewFieldInfo_x002B__relationalTableName href=""#ref-9""/>
				<ViewFieldInfo_x002B__relationalTableColumnName href=""#ref-9""/>
				<ViewFieldInfo_x002B__relationalTableColumnName2 href=""#ref-9""/>
				<ViewFieldInfo_x002B__parentReflectionType>Empty</ViewFieldInfo_x002B__parentReflectionType>
				<ViewFieldInfo_x002B__reflectedFieldArtifactTypeTableName href=""#ref-9""/>
				<ViewFieldInfo_x002B__reflectedFieldIdentifierColumnName href=""#ref-9""/>
				<ViewFieldInfo_x002B__reflectedFieldConnectorFieldName href=""#ref-9""/>
				<ViewFieldInfo_x002B__reflectedConnectorIdentifierColumnName href=""#ref-9""/>
				<ViewFieldInfo_x002B__enableDataGrid>false</ViewFieldInfo_x002B__enableDataGrid>
				<ViewFieldInfo_x002B__isVirtualAssociativeArtifactType>false</ViewFieldInfo_x002B__isVirtualAssociativeArtifactType>
				</a1:ViewFieldInfo>
				<a1:Pair id=""ref-21"" xmlns:a1=""http://schemas.microsoft.com/clr/nsassem/kCura.WinEDDS/kCura.WinEDDS%2C%20Version%3D10.2.99.73%2C%20Culture%3Dneutral%2C%20PublicKeyToken%3Dnull"">
				<Value id=""ref-33"">*ProductionArtifactId*</Value>
				<Display href=""#ref-9""/>
				</a1:Pair>
				<a1:ViewFieldInfo id=""ref-22"" xmlns:a1=""http://schemas.microsoft.com/clr/nsassem/kCura.WinEDDS/kCura.WinEDDS%2C%20Version%3D10.2.99.73%2C%20Culture%3Dneutral%2C%20PublicKeyToken%3Dnull"">
				<_fieldArtifactId>1039039</_fieldArtifactId>
				<_avfId>1001460</_avfId>
				<_category>MultiReflected</_category>
				<_connectorFieldCategory>Generic</_connectorFieldCategory>
				<_displayName id=""ref-34"">Production::Begin Bates</_displayName>
				<_avfColumnName id=""ref-35"">ProductionBeginBates</_avfColumnName>
				<_avfHeaderName id=""ref-36"">Production::Begin Bates</_avfHeaderName>
				<_allowFieldName href=""#ref-9""/>
				<_columnSource>MainTable</_columnSource>
				<_dataSource id=""ref-37"">ProductionInformation</_dataSource>
				<_sourceFieldName id=""ref-38"">Begin Bates</_sourceFieldName>
				<_sourceFieldArtifactTypeID>1000036</_sourceFieldArtifactTypeID>
				<_sourceFieldArtifactID>1039038</_sourceFieldArtifactID>
				<_connectorFieldArtifactID>1039007</_connectorFieldArtifactID>
				<_sourceFieldArtifactTypeTableName id=""ref-39"">ProductionInformation</_sourceFieldArtifactTypeTableName>
				<_connectorFieldName id=""ref-40"">Name</_connectorFieldName>
				<_fieldType>Varchar</_fieldType>
				<_isLinked>false</_isLinked>
				<_fieldCodeTypeID>-1</_fieldCodeTypeID>
				<_artifactTypeID>10</_artifactTypeID>
				<_artifactTypeTableName id=""ref-41"">Document</_artifactTypeTableName>
				<_fieldIsArtifactBaseField>false</_fieldIsArtifactBaseField>
				<_formatString href=""#ref-9""/>
				<_isUnicodeEnabled>true</_isUnicodeEnabled>
				<_allowHtml>false</_allowHtml>
				<_parentFileFieldArtifactID>-1</_parentFileFieldArtifactID>
				<_parentFileFieldDisplayName href=""#ref-9""/>
				<_associativeArtifactTypeID>1000036</_associativeArtifactTypeID>
				<_relationalTableName id=""ref-42"">f1039007f1039008</_relationalTableName>
				<_relationalTableColumnName id=""ref-43"">f1039007ArtifactID</_relationalTableColumnName>
				<_relationalTableColumnName2 id=""ref-44"">f1039008ArtifactID</_relationalTableColumnName2>
				<_parentReflectionType>Empty</_parentReflectionType>
				<_reflectedFieldArtifactTypeTableName href=""#ref-9""/>
				<_reflectedFieldIdentifierColumnName href=""#ref-9""/>
				<_reflectedFieldConnectorFieldName href=""#ref-9""/>
				<_reflectedConnectorIdentifierColumnName href=""#ref-9""/>
				<_enableDataGrid>false</_enableDataGrid>
				<_isVirtualAssociativeArtifactType>false</_isVirtualAssociativeArtifactType>
				<ViewFieldInfo_x002B__fieldArtifactId>1039039</ViewFieldInfo_x002B__fieldArtifactId>
				<ViewFieldInfo_x002B__avfId>1001460</ViewFieldInfo_x002B__avfId>
				<ViewFieldInfo_x002B__category>MultiReflected</ViewFieldInfo_x002B__category>
				<ViewFieldInfo_x002B__connectorFieldCategory>Generic</ViewFieldInfo_x002B__connectorFieldCategory>
				<ViewFieldInfo_x002B__displayName href=""#ref-34""/>
				<ViewFieldInfo_x002B__avfColumnName href=""#ref-35""/>
				<ViewFieldInfo_x002B__avfHeaderName href=""#ref-36""/>
				<ViewFieldInfo_x002B__allowFieldName href=""#ref-9""/>
				<ViewFieldInfo_x002B__columnSource>MainTable</ViewFieldInfo_x002B__columnSource>
				<ViewFieldInfo_x002B__dataSource href=""#ref-37""/>
				<ViewFieldInfo_x002B__sourceFieldName href=""#ref-38""/>
				<ViewFieldInfo_x002B__sourceFieldArtifactTypeID>1000036</ViewFieldInfo_x002B__sourceFieldArtifactTypeID>
				<ViewFieldInfo_x002B__sourceFieldArtifactID>1039038</ViewFieldInfo_x002B__sourceFieldArtifactID>
				<ViewFieldInfo_x002B__connectorFieldArtifactID>1039007</ViewFieldInfo_x002B__connectorFieldArtifactID>
				<ViewFieldInfo_x002B__sourceFieldArtifactTypeTableName href=""#ref-39""/>
				<ViewFieldInfo_x002B__connectorFieldName href=""#ref-40""/>
				<ViewFieldInfo_x002B__fieldType>Varchar</ViewFieldInfo_x002B__fieldType>
				<ViewFieldInfo_x002B__isLinked>false</ViewFieldInfo_x002B__isLinked>
				<ViewFieldInfo_x002B__fieldCodeTypeID>-1</ViewFieldInfo_x002B__fieldCodeTypeID>
				<ViewFieldInfo_x002B__artifactTypeID>10</ViewFieldInfo_x002B__artifactTypeID>
				<ViewFieldInfo_x002B__artifactTypeTableName href=""#ref-41""/>
				<ViewFieldInfo_x002B__fieldIsArtifactBaseField>false</ViewFieldInfo_x002B__fieldIsArtifactBaseField>
				<ViewFieldInfo_x002B__formatString href=""#ref-9""/>
				<ViewFieldInfo_x002B__isUnicodeEnabled>true</ViewFieldInfo_x002B__isUnicodeEnabled>
				<ViewFieldInfo_x002B__allowHtml>false</ViewFieldInfo_x002B__allowHtml>
				<ViewFieldInfo_x002B__parentFileFieldArtifactID>-1</ViewFieldInfo_x002B__parentFileFieldArtifactID>
				<ViewFieldInfo_x002B__parentFileFieldDisplayName href=""#ref-9""/>
				<ViewFieldInfo_x002B__associativeArtifactTypeID>1000036</ViewFieldInfo_x002B__associativeArtifactTypeID>
				<ViewFieldInfo_x002B__relationalTableName href=""#ref-42""/>
				<ViewFieldInfo_x002B__relationalTableColumnName href=""#ref-43""/>
				<ViewFieldInfo_x002B__relationalTableColumnName2 href=""#ref-44""/>
				<ViewFieldInfo_x002B__parentReflectionType>Empty</ViewFieldInfo_x002B__parentReflectionType>
				<ViewFieldInfo_x002B__reflectedFieldArtifactTypeTableName href=""#ref-9""/>
				<ViewFieldInfo_x002B__reflectedFieldIdentifierColumnName href=""#ref-9""/>
				<ViewFieldInfo_x002B__reflectedFieldConnectorFieldName href=""#ref-9""/>
				<ViewFieldInfo_x002B__reflectedConnectorIdentifierColumnName href=""#ref-9""/>
				<ViewFieldInfo_x002B__enableDataGrid>false</ViewFieldInfo_x002B__enableDataGrid>
				<ViewFieldInfo_x002B__isVirtualAssociativeArtifactType>false</ViewFieldInfo_x002B__isVirtualAssociativeArtifactType>
				</a1:ViewFieldInfo>
				<a1:ViewFieldInfo id=""ref-23"" xmlns:a1=""http://schemas.microsoft.com/clr/nsassem/kCura.WinEDDS/kCura.WinEDDS%2C%20Version%3D10.2.99.73%2C%20Culture%3Dneutral%2C%20PublicKeyToken%3Dnull"">
				<_fieldArtifactId>1039041</_fieldArtifactId>
				<_avfId>1001462</_avfId>
				<_category>MultiReflected</_category>
				<_connectorFieldCategory>Generic</_connectorFieldCategory>
				<_displayName id=""ref-45"">Production::End Bates</_displayName>
				<_avfColumnName id=""ref-46"">ProductionEndBates</_avfColumnName>
				<_avfHeaderName id=""ref-47"">Production::End Bates</_avfHeaderName>
				<_allowFieldName href=""#ref-9""/>
				<_columnSource>MainTable</_columnSource>
				<_dataSource id=""ref-48"">ProductionInformation</_dataSource>
				<_sourceFieldName id=""ref-49"">End Bates</_sourceFieldName>
				<_sourceFieldArtifactTypeID>1000036</_sourceFieldArtifactTypeID>
				<_sourceFieldArtifactID>1039040</_sourceFieldArtifactID>
				<_connectorFieldArtifactID>1039007</_connectorFieldArtifactID>
				<_sourceFieldArtifactTypeTableName id=""ref-50"">ProductionInformation</_sourceFieldArtifactTypeTableName>
				<_connectorFieldName id=""ref-51"">Name</_connectorFieldName>
				<_fieldType>Varchar</_fieldType>
				<_isLinked>false</_isLinked>
				<_fieldCodeTypeID>-1</_fieldCodeTypeID>
				<_artifactTypeID>10</_artifactTypeID>
				<_artifactTypeTableName id=""ref-52"">Document</_artifactTypeTableName>
				<_fieldIsArtifactBaseField>false</_fieldIsArtifactBaseField>
				<_formatString href=""#ref-9""/>
				<_isUnicodeEnabled>true</_isUnicodeEnabled>
				<_allowHtml>false</_allowHtml>
				<_parentFileFieldArtifactID>-1</_parentFileFieldArtifactID>
				<_parentFileFieldDisplayName href=""#ref-9""/>
				<_associativeArtifactTypeID>1000036</_associativeArtifactTypeID>
				<_relationalTableName id=""ref-53"">f1039007f1039008</_relationalTableName>
				<_relationalTableColumnName id=""ref-54"">f1039007ArtifactID</_relationalTableColumnName>
				<_relationalTableColumnName2 id=""ref-55"">f1039008ArtifactID</_relationalTableColumnName2>
				<_parentReflectionType>Empty</_parentReflectionType>
				<_reflectedFieldArtifactTypeTableName href=""#ref-9""/>
				<_reflectedFieldIdentifierColumnName href=""#ref-9""/>
				<_reflectedFieldConnectorFieldName href=""#ref-9""/>
				<_reflectedConnectorIdentifierColumnName href=""#ref-9""/>
				<_enableDataGrid>false</_enableDataGrid>
				<_isVirtualAssociativeArtifactType>false</_isVirtualAssociativeArtifactType>
				<ViewFieldInfo_x002B__fieldArtifactId>1039041</ViewFieldInfo_x002B__fieldArtifactId>
				<ViewFieldInfo_x002B__avfId>1001462</ViewFieldInfo_x002B__avfId>
				<ViewFieldInfo_x002B__category>MultiReflected</ViewFieldInfo_x002B__category>
				<ViewFieldInfo_x002B__connectorFieldCategory>Generic</ViewFieldInfo_x002B__connectorFieldCategory>
				<ViewFieldInfo_x002B__displayName href=""#ref-45""/>
				<ViewFieldInfo_x002B__avfColumnName href=""#ref-46""/>
				<ViewFieldInfo_x002B__avfHeaderName href=""#ref-47""/>
				<ViewFieldInfo_x002B__allowFieldName href=""#ref-9""/>
				<ViewFieldInfo_x002B__columnSource>MainTable</ViewFieldInfo_x002B__columnSource>
				<ViewFieldInfo_x002B__dataSource href=""#ref-48""/>
				<ViewFieldInfo_x002B__sourceFieldName href=""#ref-49""/>
				<ViewFieldInfo_x002B__sourceFieldArtifactTypeID>1000036</ViewFieldInfo_x002B__sourceFieldArtifactTypeID>
				<ViewFieldInfo_x002B__sourceFieldArtifactID>1039040</ViewFieldInfo_x002B__sourceFieldArtifactID>
				<ViewFieldInfo_x002B__connectorFieldArtifactID>1039007</ViewFieldInfo_x002B__connectorFieldArtifactID>
				<ViewFieldInfo_x002B__sourceFieldArtifactTypeTableName href=""#ref-50""/>
				<ViewFieldInfo_x002B__connectorFieldName href=""#ref-51""/>
				<ViewFieldInfo_x002B__fieldType>Varchar</ViewFieldInfo_x002B__fieldType>
				<ViewFieldInfo_x002B__isLinked>false</ViewFieldInfo_x002B__isLinked>
				<ViewFieldInfo_x002B__fieldCodeTypeID>-1</ViewFieldInfo_x002B__fieldCodeTypeID>
				<ViewFieldInfo_x002B__artifactTypeID>10</ViewFieldInfo_x002B__artifactTypeID>
				<ViewFieldInfo_x002B__artifactTypeTableName href=""#ref-52""/>
				<ViewFieldInfo_x002B__fieldIsArtifactBaseField>false</ViewFieldInfo_x002B__fieldIsArtifactBaseField>
				<ViewFieldInfo_x002B__formatString href=""#ref-9""/>
				<ViewFieldInfo_x002B__isUnicodeEnabled>true</ViewFieldInfo_x002B__isUnicodeEnabled>
				<ViewFieldInfo_x002B__allowHtml>false</ViewFieldInfo_x002B__allowHtml>
				<ViewFieldInfo_x002B__parentFileFieldArtifactID>-1</ViewFieldInfo_x002B__parentFileFieldArtifactID>
				<ViewFieldInfo_x002B__parentFileFieldDisplayName href=""#ref-9""/>
				<ViewFieldInfo_x002B__associativeArtifactTypeID>1000036</ViewFieldInfo_x002B__associativeArtifactTypeID>
				<ViewFieldInfo_x002B__relationalTableName href=""#ref-53""/>
				<ViewFieldInfo_x002B__relationalTableColumnName href=""#ref-54""/>
				<ViewFieldInfo_x002B__relationalTableColumnName2 href=""#ref-55""/>
				<ViewFieldInfo_x002B__parentReflectionType>Empty</ViewFieldInfo_x002B__parentReflectionType>
				<ViewFieldInfo_x002B__reflectedFieldArtifactTypeTableName href=""#ref-9""/>
				<ViewFieldInfo_x002B__reflectedFieldIdentifierColumnName href=""#ref-9""/>
				<ViewFieldInfo_x002B__reflectedFieldConnectorFieldName href=""#ref-9""/>
				<ViewFieldInfo_x002B__reflectedConnectorIdentifierColumnName href=""#ref-9""/>
				<ViewFieldInfo_x002B__enableDataGrid>false</ViewFieldInfo_x002B__enableDataGrid>
				<ViewFieldInfo_x002B__isVirtualAssociativeArtifactType>false</ViewFieldInfo_x002B__isVirtualAssociativeArtifactType>
				</a1:ViewFieldInfo>
				<a3:FirstFieldDescriptorPart id=""ref-24"" xmlns:a3=""http://schemas.microsoft.com/clr/nsassem/FileNaming.CustomFileNaming/kCura.WinEDDS%2C%20Version%3D10.2.99.73%2C%20Culture%3Dneutral%2C%20PublicKeyToken%3Dnull"">
				<_IsProduction>false</_IsProduction>
				<FieldDescriptorPart_x002B__Value>1003667</FieldDescriptorPart_x002B__Value>
				</a3:FirstFieldDescriptorPart>
				<a4:ArrayList id=""ref-25"" xmlns:a4=""http://schemas.microsoft.com/clr/ns/System.Collections"">
				<_items href=""#ref-56""/>
				<_size>0</_size>
				<_version>0</_version>
				</a4:ArrayList>
				<SOAP-ENC:Array id=""ref-56"" SOAP-ENC:arrayType=""xsd:anyType[2]"">
				</SOAP-ENC:Array>
				</SOAP-ENV:Body>
				</SOAP-ENV:Envelope>
				";
		}
	}
}
