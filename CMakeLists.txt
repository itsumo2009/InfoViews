cmake_minimum_required(VERSION 3.8.2)
set(PROJECT_NAME InfoViews)
project(${PROJECT_NAME} VERSION 0.1.0 LANGUAGES CSharp)

set(CITIES_STEAM_ID 255710)
set(INFO_VIEW_STEAM_ID 1192503086)

file(GLOB_RECURSE SRCS *.cs)
file(GLOB_RECURSE RES Resources/*)
file(GLOB_RECURSE ICONS Icon/*)

if(WIN32)
    set(MOD_INSTALL_INITAL_PATH "$ENV{LOCALAPPDATA}/Colossal Order/Cities_Skylines/Addons/Mods/${PROJECT_NAME}")
elseif(APPLE)
    set(MOD_INSTALL_INITAL_PATH "$ENV{HOME}//Library/Application Support/Colossal Order/Cities_Skylines/Addons/Mods/${PROJECT_NAME}" )
else()
    set(MOD_INSTALL_INITAL_PATH "$ENV{HOME}/.local/share/Colossal Order/Cities_Skylines/Addons/Mods/${PROJECT_NAME}" )
endif()

set(MOD_INSTALL_PATH "${MOD_INSTALL_INITAL_PATH}" CACHE PATH "Path for publishing mod")

set(CITIES_STEAM_LIBRARY "C:/Program Files (x86)/Steam" CACHE PATH "Path to steam library which contents Cities:Skylines")
find_path(CITIES_MOD_PARENT_PATH 
          ${CITIES_STEAM_ID}
          PATHS "${CITIES_STEAM_LIBRARY}/steamapps/workshop/content"
          DOC "Path to InfoViews installation")

#set(MOD_PATH "${CITIES_MOD_PARENT_PATH}/${CITIES_STEAM_ID}/${INFO_VIEW_STEAM_ID}") 
set(MOD_PATH "${MOD_INSTALL_PATH}")#/${CITIES_STEAM_ID}/${INFO_VIEW_STEAM_ID}") 

add_library(${PROJECT_NAME} ${SRCS} ${RES})

find_path(CITIES_MANAGED_PATH
    ICities.dll
    PATHS "${CITIES_STEAM_LIBRARY}/steamapps/common/Cities_Skylines/Cities_Data/Managed" DOC "Path to Cities Skylines managed libs")

set(REFS
    System
    System.Core
    System.Xml.Linq
    System.Data.DataSetExtensions
    System.Data
    System.Net.Http
    System.Xml
    )

set(CITIES_REFS
    Assembly-CSharp
    Assembly-CSharp-firstpass
    ColossalManaged
    ICities
    ICSharpCode.SharpZipLib
    Mono.Posix
    Mono.Security
    UnityEngine
    UnityEngine.Networking
    UnityEngine.UI
)

set_property(TARGET ${PROJECT_NAME} PROPERTY VS_DOTNET_REFERENCES ${REFS})

foreach(CITIES_REF IN ITEMS ${CITIES_REFS})
    set_target_properties(${PROJECT_NAME} PROPERTIES VS_DOTNET_REFERENCE_${CITIES_REF} "${CITIES_MANAGED_PATH}/${CITIES_REF}.dll")
endforeach()
set_property(TARGET ${PROJECT_NAME} PROPERTY VS_DOTNET_REFERENCES ${REFS})

foreach(CITIES_REF IN ITEMS ${CITIES_REFS})
	set_target_properties(${PROJECT_NAME} PROPERTIES VS_DOTNET_REFERENCE_${CITIES_REF} "${CITIES_MANAGED_PATH}/${CITIES_REF}.dll")
endforeach()

set_target_properties(${PROJECT_NAME} PROPERTIES VS_DOTNET_REFERENCE_0Harmony "${CMAKE_SOURCE_DIR}/packages/0Harmony.dll")

target_compile_options(${PROJECT_NAME} PRIVATE "/langversion:6" "/unsafe" "/platform:anycpu")

set_target_properties(${PROJECT_NAME} PROPERTIES
  VS_GLOBAL_ROOTNAMESPACE ${PROJECT_NAME}
  VS_GLOBAL_ProjectGuid "{f03f9a9f-5f7b-4d30-97fb-1d46cca132ce}"
  VS_GLOBAL_CodeAnalysisRuleSet "${CMAKE_SOURCE_DIR}/InfoViews.ruleset"
  VS_GLOBAL_FileAlignment "512"
  VS_GLOBAL_Prefer32Bit "false"
  VS_GLOBAL_LangVersion "7.3"
  VS_GLOBAL_AllowUnsafeBlocks "true"
  VS_GLOBAL_Platform "AnyCPU"
  VS_GLOBAL_WarningLevel "4"
  VS_GLOBAL_PlatformTarget "AnyCPU"
  )

set_property(TARGET ${PROJECT_NAME} PROPERTY VS_DOTNET_TARGET_FRAMEWORK_VERSION "v3.5")
set_property(TARGET ${PROJECT_NAME} PROPERTY VS_DOTNET_REFERENCES_COPY_LOCAL False)

function(add_mod_file SRC DEST)
	add_custom_command(TARGET ${PROJECT_NAME} POST_BUILD COMMAND "${CMAKE_COMMAND}" -E copy_if_different "${SRC}" "${DEST}" COMMENT "Copying ${SRC} -> ${DEST}\r")
	#install(FILES "${DEST}" DESTINATION "${MOD_INSTALL_PATH}")
endfunction(add_mod_file)

function(add_mod_directory SRC DEST)
	add_custom_command(TARGET ${PROJECT_NAME} POST_BUILD COMMAND "${CMAKE_COMMAND}" -E copy_directory "${SRC}" "${DEST}" COMMENT "Copying ${SRC} -> ${DEST}\r")
	#install(DIRECTORY "${DEST}" DESTINATION  "${MOD_INSTALL_PATH}")
endfunction(add_mod_directory)

add_mod_directory("${CMAKE_SOURCE_DIR}/Icon" "${MOD_PATH}/Icon")

add_mod_file("${CMAKE_BINARY_DIR}/${CMAKE_CFG_INTDIR}/${PROJECT_NAME}.dll" "${MOD_PATH}/${PROJECT_NAME}.dll")
add_mod_file("${CMAKE_SOURCE_DIR}/packages/0Harmony.dll" "${MOD_PATH}/0Harmony.dll")
