using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using JetBrains.Annotations;

/**
 * ENUMS
 */

public enum AnnexDataCategory
{
 [EnumMember(Value = "Site Inspection")]
 site_inspection,
 [EnumMember(Value = "Historical Data")]
 historical_data,
 [EnumMember(Value = "Survey Data")]
 survey_data
}

public enum AnnotationType
{
 [EnumMember(Value = "2D")]
 _2D,
 [EnumMember(Value = "3D")]
 _3D
}

public enum HeritageCategory
{
 Movable,
 Immovable
}

public enum ScaleType
{
 Building,
 Object
}

public enum SpatialDimension
{
 _2D,
 _3D
}

public enum CurrentUseType
{
 [EnumMember(Value = "Not used")]
 notused,
 [EnumMember(Value = "Used, visiting site")]
 visitingsite,
 [EnumMember(Value = "Used, worship site")]
 worshipsite,
 [EnumMember(Value = "Used, office use")]
 officeuse,
 [EnumMember(Value = "Other use")]
 otheruse
}

public enum StateOfConservationType
{
 [EnumMember(Value = "Good")]
 good,
 [EnumMember(Value = "Fair")]
 fair,
 [EnumMember(Value = "Poor")]
 poor,
 [EnumMember(Value = "Very Bad")]
 verybad
}

public enum StatusType
{
 [EnumMember(Value = "Draft")]
 draft,
 [EnumMember(Value = "Finished")]
 finished,
 [EnumMember(Value = "Public")]
 _public
}

public enum SearchType
{
 E3DModel,
 E3DInstance,
 Annotation,
 CHElement,
 Unknown
}

public enum InclusionType
{
 [EnumMember(Value = "Part of")]
 part_of,
 [EnumMember(Value = "Inside of")]
 inside_of
}

public enum TextureType
{
 [EnumMember(Value = "RGB")]
 RGB,
 [EnumMember(Value = "IR")]
 IR,
 [EnumMember(Value = "RTI")]
 RTI,
 [EnumMember(Value = "Hyper Spectral")]
 hyperspectral
}

public enum DetailLevel
{
 [EnumMember(Value = "Cloud Points")]
 cloudpoints,
 [EnumMember(Value = "High Poly")]
 highpoly,
 [EnumMember(Value = "Medium Poly")]
 mediumpoly,
 [EnumMember(Value = "Low Poly")]
 lowpoly
}

public enum Format
{
 [EnumMember(Value = "PLY")]
 PLY,
 [EnumMember(Value = "GLTF")]
 GLTF
}

public enum LicenseType
{
 [EnumMember(Value = "CC 0")]
 CC0,
 [EnumMember(Value = "CC BY")]
 CCBY,
 [EnumMember(Value = "CC BY-SA")]
 CCBY_SA,
 [EnumMember(Value = "CC BY-NC")]
 CCBY_NC,
 [EnumMember(Value = "CC BY-NC-SA")]
 CCBY_NC_SA,
 [EnumMember(Value = "CC BY-ND")]
 CCBY_ND,
 [EnumMember(Value = "CC BY-NC-ND")]
 CCBY_NC_ND
}

public enum FileTypeCategory
{
 [EnumMember(Value = "Annex Data")]
 annex_data,
 [EnumMember(Value = "Annotation")]
 annotation,
 [EnumMember(Value = "Condition Report")]
 condition_report,
 [EnumMember(Value = "Texture")]
 texture,
 [EnumMember(Value = "E3D Instance")]
 e3d_instance,
 [EnumMember(Value = "Icon")]
 icon,
 [EnumMember(Value = "Texture Layer")]
 texture_layer
}

public enum UserRole
{
 [EnumMember(Value = "Administrator")]
 admin,
 [EnumMember(Value = "Moderator")]
 moderator,
 [EnumMember(Value = "Specialist")]
 specialist,
 [EnumMember(Value = "Visitor")]
 visitor
}

/**
 * ALTERATION AGENT CLASSES
 */

[Serializable]
public class AlterationAgentPost
{
    public int? id_alteration_agent_type;
    public string name;
}

[Serializable]
public class AlterationAgentData
{
    public string GUID;
    public int id;
    public int id_alteration_agent_type;
    public string name;
    public string error;
}

[Serializable]
public class AlterationAgentResponse
{
    public List<AlterationAgentData> items;
    public int? next_page;
    public int? page;
    public int? pages;
    public int? per_page;
    public int? prev_page;
    public int? total;
    public string error;
}

/**
 * ALTERATION AGENT TYPE CLASSES
 */

[Serializable]
public class AlterationAgentTypePost
{
 public string name;
}

[Serializable]
public class AlterationAgentTypeData
{
 public string GUID;
 public int id;
 public string name;
 public string error;
}

[Serializable]
public class AlterationAgentTypeResponse
{
 public List<AlterationAgentTypeData> items;
 public int? next_page;
 public int? page;
 public int? pages;
 public int? per_page;
 public int? prev_page;
 public int? total;
 public string error;
}
 
/**
* ALTERATION CLASSES
*/

[Serializable]
public class AlterationPost
{
 public int? id_alteration_form;
 public string name;
}

[Serializable]
public class AlterationData
{
 public string GUID;
 public int id;
 public int id_alteration_form;
 public string name;
 public string error;
}

[Serializable]
public class AlterationResponse
{
 public List<AlterationData> items;
 public int? next_page;
 public int? page;
 public int? pages;
 public int? per_page;
 public int? prev_page;
 public int? total;
 public string error;
}

/**
 * ALTERATION EVENT CLASSES
 */

[Serializable]
public class AlterationEventPost
{
 public string description;
 public int? id_alteration_agent;
 [CanBeNull] public List<int> id_alterations;
 [CanBeNull] public List<int> id_annex_data;
 public int? id_material;
 public string name;
}

[Serializable]
public class AlterationEventData
{
 public string GUID;
 public List<AlterationData> alterations;
 public List<string> annex_data;
 public string created_on;
 public string description;
 public int id;
 public int? id_alteration_agent;
 public string name;
 public string error;
}

[Serializable]
public class AlterationEventResponse
{
 public List<AlterationEventData> items;
 public int? next_page;
 public int? page;
 public int? pages;
 public int? per_page;
 public int? prev_page;
 public int? total;
 public string error;
}
 
/**
* ALTERATION FORM CLASSES
*/

[Serializable]
public class AlterationFormPost
{
 public string name;
}

[Serializable]
public class AlterationFormData
{
 public string GUID;
 public List<AlterationData> alterations;
 public int id;
 public string name;
 public string error;
}

[Serializable]
public class AlterationFormResponse
{
 public List<AlterationFormData> items;
 public int? next_page;
 public int? page;
 public int? pages;
 public int? per_page;
 public int? prev_page;
 public int? total;
 public string error;
}

/**
 * ANNEX DATA CLASSES
 */

[Serializable]
public class Doc
{
 public string content_type;
 public int id;
 public string name;

 public Doc(string contentType, int id, string name)
 {
  content_type = contentType;
  this.id = id;
  this.name = name;
 }
}

[Serializable]
public class ParentAnnexData
{
 public string entity_type;
 public int id;
 public string name;
 public ParentCHElement parent; // nested parent

 public ParentAnnexData(string entityType, int id, string name, ParentCHElement parent)
 {
  entity_type = entityType;
  this.id = id;
  this.name = name;
  this.parent = parent;
 }
}

[Serializable]
public class AnnexDataPost
{
 public string category;
 public string content;
 public int? icon;
 [CanBeNull] public List<int> id_annex_docs;
 public int? id_chelement;
 public int? id_e3dmodel;
 public int? id_site;
 public string timestamp;
 public string title;
}

[Serializable]
public class AnnexDataData
{
 public string GUID;
 public string category;
 public string content;
 public string created_on;
 public List<Doc> docs;
 public int? icon;
 public string icon_path;
 public int id;
 [CanBeNull] public List<int> id_annex_docs;
 public int? id_chelement;
 public int? id_e3dmodel;
 public int? id_site;
 public int id_user;
 public object parent;
 public string timestamp;
 public string title;
 public string username;
 public string error;
}

[Serializable]
public class AnnexDataResponse
{
 public List<AnnexDataData> items;
 public int? next_page;
 public int? page;
 public int? pages;
 public int? per_page;
 public int? prev_page;
 public int? total;
 public string error;
}
 
/**
* ANNEX DATA FILE CLASSES
*/

[Serializable]
public class AnnexDataFilePost
{
 public int id_file;
}

[Serializable]
public class AnnexDataFileData
{
 public int id;
 public int id_annex_data;
 public int id_file;
 public string error;
}

[Serializable]
public class AnnexDataFileResponse
{
 public List<AnnexDataFileData> items;
 public int? next_page;
 public int? page;
 public int? pages;
 public int? per_page;
 public int? prev_page;
 public int? total;
 public string error;
}

/**
 * ANNOTATION CLASSES
 */

[Serializable]
public class AnnotationInfo
{
 public string annotation_info_name;
 public string created_on;
 public int file;
 public int id;
 public int id_annotation;
 public string type;

 public AnnotationInfo(string annotationInfoName, string createdOn, int file, int id,
  int idAnnotation, string type)
 {
  annotation_info_name = annotationInfoName;
  created_on = createdOn;
  this.file = file;
  this.id = id;
  id_annotation = idAnnotation;
  this.type = type;
 }
}

[Serializable]
public class AnnotationPost
{
 public List<AnnotationInfo> annotation_infos;
 public string description;
 public int? icon; // null in JSON
 public int? id_group; // null in JSON, possibly int?
 [CanBeNull] public List<int> id_textures;
 public string name;
 public int? shape_and_context; // null in JSON
 public string type;
}

[Serializable]
public class AnnotationData
{
 public string GUID;
 public List<AnnotationInfo> annotation_infos;
 public string created_on;
 public string description;
 public int? icon; // null in JSON
 public string icon_path;
 public int id;
 public int id_3dinstance;
 public int? id_group; // null in JSON, possibly int?
 public int id_user;
 public string name;
 public ParentThreeDInstance parent;
 public int? shape_and_context; // null in JSON
 public string shape_and_context_name;
 public List<TextureInfo> texture_layers;
 public string type;
 public string username;
 public string error;
}

[Serializable]
public class AnnotationResponse
{
 public List<AnnotationData> items;
 public int? next_page;
 public int? page;
 public int? pages;
 public int? per_page;
 public int? prev_page;
 public int? total;
 public string error;
}
 
/**
* ANNOTATION GROUP CLASSES
*/

[Serializable]
public class AnnotationGroupPost
{
 public string description;
 [CanBeNull] public List<int> id_annotations;
 public string name;
}

[Serializable]
public class AnnotationGroupData
{
 public string GUID;
 public string created_on;
 public string description;
 public int id;
 public int id_user;
 public string name;
 public string username;
 public string error;
}

[Serializable]
public class AnnotationGroupResponse
{
 public List<AnnotationGroupData> items;
 public int? next_page;
 public int? page;
 public int? pages;
 public int? per_page;
 public int? prev_page;
 public int? total;
 public string error;
}

/**
 * CHELEMENT CLASSES
 */

[Serializable]
public class Count
{
    public int entity_no;
    public string entity_type;
    
    public Count(int entityNo, string entityType)
    {
        entity_no = entityNo;
        entity_type = entityType;
    }
}

[Serializable]
public class ChElementMaterial
{
    public string description;
    public int id;
    
    public ChElementMaterial(string description, int id)
    {
        this.description = description;
        this.id = id;
    }
}

[Serializable]
public class ParentCHElement
{
    public string entity_type;
    public int id;
    public string name;
    
    public ParentCHElement(string entityType, int id, string name)
    {
        entity_type = entityType;
        this.id = id;
        this.name = name;
    }
}

[Serializable]
public class CHElementPost
{
    public string address;
    public bool? approximate_date;
    public string author;
    [CanBeNull] public List<ChElementMaterial> chelement_materials;
    public string cultural_sphere;
    public string description;
    public int? end_year;
    public string heritage_category;
    public int? icon; // null in JSON, but use string for flexibility
    public int? id_country;
    public int? id_municipality;
    public int? id_province;
    public int? id_site;
    public float? latitude;
    public float? longitude;
    public string name;
    public string scale;
    public string spatial_dimension;
    public int? start_year;
}

[Serializable]
public class CHElementData
{
    public string GUID;
    public string address;
    public bool approximate_date;
    public string author;
    public GeoData country;
    public List<Count> counts;
    public string created_on; // You can convert to DateTime manually if needed
    public string cultural_sphere;
    public string description;
    public int end_year;
    public string heritage_category;
    public int? icon; // Kept as string to handle nulls
    public string icon_path;
    public int id;
    public int? id_country;
    public int? id_municipality;
    public int? id_province;
    public int id_user;
    public float? latitude;
    public float? longitude;
    public List<MaterialData> materials;
    public GeoData municipality;
    public string name;
    [CanBeNull] public ParentCHElement parent;
    public GeoData province;
    public string scale;
    public string spatial_dimension;
    public int start_year;
    public string state_of_conservation;
    public string username;
    public string error;
}

[Serializable]
public class CHElementResponse
{
    public List<CHElementData> items;
    public int? next_page;
    public int? page;
    public int? pages;
    public int? per_page;
    public int? prev_page;
    public int? total;
    public string error;
}
 
/**
* CHUSAGE CLASSES
*/

[Serializable]
public class ChUsageDescription
{
 public int ch_usage_id;
 public string description;
}

[Serializable]
public class Use
{
 public CHUsageData ch_usage;
 public string description;
}

[Serializable]
public class CHUsageData
{
 public int id;
 public string name;
}

[Serializable]
public class CHUsageResponse
{
 public List<CHUsageData> items;
 public int? next_page;
 public int? page;
 public int? pages;
 public int? per_page;
 public int? prev_page;
 public int? total;
 public string error;
}

/**
 * CLIENT CLASSES
 */

[Serializable]
public class ClientPost
{
 public string client_name;
 public string client_uri;
 public string redirect_uris;
}

[Serializable]
public class ClientData
{
 public int? client_id;
 public string client_metadata;
 public string client_secret;
 public int? id;
 public string error;
}
 
/**
* CONDITION REPORT CLASSES
*/

[Serializable]
public class ConditionReportPost
{
    public string address;
    public string author;
    [CanBeNull] public List<ChInspectionMode> ch_inspection_modes;
    [CanBeNull] public List<ChUsageDescription> ch_usages;
    public string colocation_characteristics;
    public string conservation_history;
    public string cultural_sphere;
    public string date;
    public string description;
    public string diagnosis_description;
    public string environmental_conditions_current_weather_condition;
    public string environmental_conditions_past_week_weather_condition;
    public string environmental_conditions_relative_humidity;
    public string environmental_conditions_temperature;
    [CanBeNull] public List<int> id_annotations;
    public string legal_status;
    public string municipality;
    public string name;
    public string object_string;
    public string parent_asset_name;
    public string province;
    public string reference_cartography_cadastral_parcel;
    public string reference_cartography_cadastral_sheet;
    public string reference_cartography_municipality;
    public string reference_cartography_other;
    public string report_references;
    public string specific_location;
    public string state_of_conservation;
    public string status;
    public string subject;
    public string survey_date;
    public string survey_responsible;
    public string techniques;
}

[Serializable]
public class ConditionReportData
{
    public string GUID;
    public string address;
    public List<AnnotationData> annotations;
    public string author;
    public string colocation_characteristics;
    public string conservation_history;
    public string created_on;
    public string cultural_sphere;
    public string date;
    public string description;
    public string diagnosis_description;
    public string enviromental_conditions_current_weather_condition;
    public string enviromental_conditions_past_week_weather_condition;
    public string enviromental_conditions_relative_humidity;
    public string enviromental_conditions_temperature;
    public int id;
    public int id_e3dmodel;
    public int id_user;
    public List<InspectionModeData> inspection_modes;
    public string legal_status;
    public string municipality;
    public string name;
    public string object_string;
    public string parent_asset_name;
    public string province;
    public string reference_cartography_cadastral_parcel;
    public string reference_cartography_cadastral_sheet;
    public string reference_cartography_municipality;
    public string reference_cartography_other;
    public string report_references;
    public string specific_location;
    public string state_of_conservation;
    public string status;
    public string subject;
    public string survey_date;
    public string survey_responsible;
    public string techniques;
    public List<Use> use;
    public string username;
    public string error;
}

[Serializable]
public class ConditionReportResponse
{
    public List<ConditionReportData> items;
    public int? next_page;
    public int? page;
    public int? pages;
    public int? per_page;
    public int? prev_page;
    public int? total;
    public string error;
}

/**
 * CONDITION REPORT FILE CLASSES
 */

[Serializable]
public class ConditionReportFilePost
{
 public int id_file;
}

[Serializable]
public class ConditionReportFileData
{
 public int id;
 public int id_condition_report;
 public int id_file;
 public string error;
}

[Serializable]
public class ConditionReportFileResponse
{
 public List<ConditionReportFileData> items;
 public int? next_page;
 public int? page;
 public int? pages;
 public int? per_page;
 public int? prev_page;
 public int? total;
 public string error;
}
 
/**
* COUNTRY CLASSES
*/

[Serializable]
public class GeoData
{
 public string admin1;
 public string admin2;
 public string asciiname;
 public string country;
 public string fcode;
 public int geonameid;
 public string name;
 public string error;
}

[Serializable]
public class CountryResponse
{
 public List<GeoData> items;
 public int? next_page;
 public int? page;
 public int? pages;
 public int? per_page;
 public int? prev_page;
 public int? total;
 public string error;
    
 public CountryResponse(List<GeoData> items, int? nextPage, int? page, int? pages, int? perPage,
  int? prevPage, int? total, string error)
 {
  this.items = items;
  next_page = nextPage;
  this.page = page;
  this.pages = pages;
  per_page = perPage;
  prev_page = prevPage;
  this.total = total;
  this.error = error;
 }
}

/**
* E3DMODEL CLASSES
*/

[Serializable]
public class ParentE3DModel
{
 public string entity_type;
 public int id;
 public string name;
 public ParentCHElement parent; // nested parent

 public ParentE3DModel(string entityType, int id, string name, ParentCHElement parent)
 {
  entity_type = entityType;
  this.id = id;
  this.name = name;
  this.parent = parent;
 }
}

[Serializable]
public class E3DModelPost
{
 public string description;
 public float? height;
 public int? icon; // null in JSON, use object for flexibility
 public int? id_chelement;
 public float? length;
 public string name;
 public string reference_date; // Use string if format is "YYYY-MM-DD"
 public float? scale;
 public float? width;
}

[Serializable]
public class E3DModelData
{
 public string GUID;
 public List<Count> counts;
 public string created_on;
 public string description;
 public float height;
 public int? icon; // null in JSON
 public string icon_path;
 public int id;
 public int id_chelement;
 public int id_user;
 public float length;
 public string name;
 public ParentAnnexData parent;
 public string reference_date; // or DateTime if parsed accordingly
 public float scale;
 public string username;
 public float width;
 public string error;
}

[Serializable]
public class E3DModelResponse
{
 public List<E3DModelData> items;
 public int? next_page;
 public int? page;
 public int? pages;
 public int? per_page;
 public int? prev_page;
 public int? total;
 public string error;
}
 
/**
* INIT CLASSES
*/

[Serializable]
public class InitResponse
{
 public string GUID;
 public string created_on;
 public string description;
 public int? icon;
 public string icon_path;
 public int id;
 public int id_user;
 public string name;
 public List<SiteData> sites;
 public string username;
 public string error;
}

/**
 * INSPECTION MODE CLASSES
 */

[Serializable]
public class ChInspectionMode
{
 public string description;
 public int inspection_mode_id;
}

[Serializable]
public class InspectionModeDescription
{
 public string description;
 public InspectionModeData inspection_mode;
}

[Serializable]
public class InspectionModeData
{
 public int id;
 public string presence;
 public string type;
}

[Serializable]
public class InspectionModeResponse
{
 public List<InspectionModeData> items;
 public int? next_page;
 public int? page;
 public int? pages;
 public int? per_page;
 public int? prev_page;
 public int? total;
 public string error;
}
 
/**
* MATERIAL CLASSES
*/

[Serializable]
public class MaterialPost
{
 public string name;
}

[Serializable]
public class MaterialData
{
 public string GUID;
 public int id;
 public string name;
 public string error;
}

[Serializable]
public class MaterialResponse
{
 public List<MaterialData> items;
 public int? next_page;
 public int? page;
 public int? pages;
 public int? per_page;
 public int? prev_page;
 public int? total;
 public string error;
}

/**
 * MUNICIPALITY CLASSES
 */

[Serializable]
public class MunicipalityResponse
{
 public List<GeoData> items;
 public int? next_page;
 public int? page;
 public int? pages;
 public int? per_page;
 public int? prev_page;
 public int? total;
 public string error;
    
 public MunicipalityResponse(List<GeoData> items, int? nextPage, int? page, int? pages, int? perPage,
  int? prevPage, int? total, string error)
 {
  this.items = items;
  next_page = nextPage;
  this.page = page;
  this.pages = pages;
  per_page = perPage;
  prev_page = prevPage;
  this.total = total;
  this.error = error;
 }
}
 
/**
* PILOT CLASSES
*/

[Serializable]
public class PilotPost
{
 public string description;
 public int? icon; // null by default, represented as int
 public List<int> id_sites;
 public string name;
}

[Serializable]
public class PilotData
{
 public string GUID;
 public string created_on;
 public string description;
 public int? icon;
 public string icon_path;
 public int id;
 public int id_user;
 public string name;
 public List<SiteData> sites;
 public string username;
 public string error;
}

[Serializable]
public class PilotResponse
{
 public List<PilotData> items;
 public int? next_page;
 public int? page;
 public int? pages;
 public int? per_page;
 public int? prev_page;
 public int? total;
 public string error;
}

/**
 * PROVINCE CLASSES
 */

[Serializable]
public class ProvinceResponse
{
 public List<GeoData> items;
 public int? next_page;
 public int? page;
 public int? pages;
 public int? per_page;
 public int? prev_page;
 public int? total;
 public string error;
    
 public ProvinceResponse(List<GeoData> items, int? nextPage, int? page, int? pages, int? perPage,
  int? prevPage, int? total, string error)
 {
  this.items = items;
  next_page = nextPage;
  this.page = page;
  this.pages = pages;
  per_page = perPage;
  prev_page = prevPage;
  this.total = total;
  this.error = error;
 }
}

/**
 * SEARCH CLASSES
 */

[Serializable]
public class SearchResponse
{
 public int? annotations_no;
 public int? chelements_no;
 public int? e3dinstances_no;
 public int? e3dmodels_no;
 public List<SearchItem> items; // Usamos object porque contiene nulls (no hay datos definidos)
 public int? next_page;
 public int? page;
 public int? pages;
 public int? per_page;
 public int? prev_page;
 public int? total;
}

[Serializable]
public class SearchItem
{
 public int? id;
 public string name;
 public string description;
 public string created_on;
 public string entity_type;
}
 
/**
* SIGNIN CLASSES
*/

[Serializable]
public class SigninPost
{
 public string password;
 public string username;
    
 public SigninPost(string password, string username)
 {
  this.password = password;
  this.username = username;
 }
}

[Serializable]
public class SigninResponse
{
 public string access_token;
 public string error;

 public SigninResponse(string accessToken, string error)
 {
  access_token = accessToken;
  this.error = error;
 }
}

/**
* SITE CLASSES
*/

[Serializable]
public class SitePost
{
 public string description;
 public int? icon; // null by default, represented as string
 public int id_country;
 public float latitude;
 public float longitude;
 public string name;
}

[Serializable]
public class SiteData
{
 public string GUID;
 public List<Count> counts;
 public string created_on;
 public string description;
 public int? icon;
 public string icon_path;
 public int id;
 public int id_country;
 public int id_user;
 public float latitude;
 public float longitude;
 public string name;
 public List<ParentCHElement> pilots;
 public string username;
 public string error;
}

[Serializable]
public class SiteResponse
{
 public List<SiteData> items;
 public int? next_page;
 public int? page;
 public int? pages;
 public int? per_page;
 public int? prev_page;
 public int? total;
 public string error;
}

/**
 * SPATIAL RELATION CLASSES
 */

[Serializable]
public class SpatialRelationPost
{
 public int? id_included;
 public string inclusion_type;
 public int? transformation;
}

[Serializable]
public class SpatialRelationData
{
 public string created_on;
 public int id;
 public int id_included;
 public int id_reference;
 public int id_user;
 public string inclusion_type;
 public int transformation;
 public string username;
 public string error;
}

[Serializable]
public class SpatialRelationResponse
{
 public List<SpatialRelationData> items;
 public int? next_page;
 public int? page;
 public int? pages;
 public int? per_page;
 public int? prev_page;
 public int? total;
 public string error;
}
 
/**
* TEXTURE LAYER CLASSES
*/

public class TextureInfo
{
 public int id;
 public string name;

 public TextureInfo(int id, string name)
 {
  this.id = id;
  this.name = name;
 }
}

[Serializable]
public class ParentTextureLayer
{
 public string entity_type;
 public int id;
 public string name;
 public ParentThreeDInstance parent; // nested parent

 public ParentTextureLayer(string entityType, int id, string name, ParentThreeDInstance parent)
 {
  entity_type = entityType;
  this.id = id;
  this.name = name;
  this.parent = parent;
 }
}

[Serializable]
public class TextureLayerPost
{
 public string config;
 public string description;
 public int? icon;
 public int? id_3Dinstance;
 public int? id_gltf_file;
 [CanBeNull] public List<int> id_textures;
 public string license_type;
 public string name;
 public string sensor_configuration;
 public string textureType;
}

[Serializable]
public class TextureLayerData
{
 public string GUID;
 public string config;
 public string created_on;
 public string description;
 public string gltf_name;
 public int? icon; // null in JSON
 public string icon_path;
 public int id;
 public int id_3Dinstance;
 public int id_gltf_file;
 public List<int> id_textures; // null in JSON, but possibly an int or list in other cases
 public int id_user;
 public string license_type;
 public string name;
 public ParentTextureLayer parent;
 public string sensor_configuration;
 public List<TextureInfo> textures;
 public string textureType;
 public string username;
 public string error;
}

[Serializable]
public class TextureLayerResponse
{
 public List<TextureLayerData> items;
 public int? next_page;
 public int? page;
 public int? pages;
 public int? per_page;
 public int? prev_page;
 public int? total;
 public string error;
}

/**
 * THREE D INSTANCE CLASSES
 */

[Serializable]
public class ParentThreeDInstance
{
 public string entity_type;
 public int id;
 public string name;
 public ParentE3DModel parent; // nested parent

 public ParentThreeDInstance(string entity_type, int id, string name, ParentE3DModel parent)
 {
  this.entity_type = entity_type;
  this.id = id;
  this.name = name;
  this.parent = parent;
 }
}

[Serializable]
public class ThreeDInstancePost
{
 public string description;
 public string detail_level;
 public string format;
 public int? icon; // null in JSON, use object for flexibility
 public int? id_e3dmodel;
 public int? id_file;
 public string license_type;
 public string name;
 public string sensor_configuration;
}

[Serializable]
public class ThreeDInstanceData
{
 public string GUID;
 public string bin_name;
 public List<Count> counts;
 public string created_on;
 public string description;
 public string detail_level;
 public string format;
 public int? icon;
 public string icon_path;
 public int id;
 public int id_e3dmodel;
 public int id_file;
 public int id_user;
 public string license_type;
 public string name;
 public string no_texture_layers;
 public ParentAnnexData parent;
 public string sensor_configuration;
 public string username;
 public string error;
}

[Serializable]
public class ThreeDInstanceResponse
{
 public List<ThreeDInstanceData> items;
 public int? next_page;
 public int? page;
 public int? pages;
 public int? per_page;
 public int? prev_page;
 public int? total;
 public string error;
}
 
/**
* UPLOAD CLASSES
*/

[Serializable]
public class UploadDataPost
{
 public string file_checksum;
 public string file_name;
 public string file_type;
 public int? id; // null in JSON, treated as nullable string
 public int? total_chunks; // nullable int
}

[Serializable]
public class UploadData
{
 public string GUID;               // nullable -> string
 public string created_on;
 public string file_checksum;
 public string file_name;
 public int? file_size;           // nullable -> long?
 public string file_type;
 public int? id;                 // nullable -> string
 public int? id_user;              // nullable -> int?
 public string status;
 public int? total_chunks;         // nullable -> int?
 public string updated_at;         // nullable -> string
 public string username;
 public string error;
}

/**
 * USER CLASSES
 */

[Serializable]
public class UserPost
{
 public int? active;
 public int? activity_start;
 public string affiliation;
 public string email;
 public int? id_country;
 public string name;
 public string password;
 public string profession;
 public string role;
 public string surname;
 public string username;
}

[Serializable]
public class UserData
{
 public int active;
 public int activity_start;
 public string affiliation;
 public string email;
 public int id;
 public int id_country;
 public string name;
 public string profession;
 public string role;
 public string surname;
 public string username;
 public string error;
}

[Serializable]
public class UserResponse
{
 public List<UserData> items;
 public int? next_page;
 public int? page;
 public int? pages;
 public int? per_page;
 public int? prev_page;
 public int? total;
 public string error;
}
 
/**
* WATCHLIST CLASSES
*/

[Serializable]
public class WatchListPost
{
 public string entity_type;
 public string guid;
}

[Serializable]
public class WatchListData
{
 public string entity_type;
 public string guid;
 public string error;
}

[Serializable]
public class WatchListResponse
{
 public List<WatchListData> items;
 public string error;
}