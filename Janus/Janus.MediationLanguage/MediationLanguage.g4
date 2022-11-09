grammar MediationLanguage;

/* LEXER RULES */
/* keywords */
/* general keywords */
TABLEAU_KW		:'TABLEAU';
SCHEMA_KW		:'SCHEMA';
DATASOURCE_KW	:'DATASOURCE';
ATTRIBUTES_KW	:'ATTRIBUTES';
AS_KW			:'AS';
BEING_KW		:'BEING';
VIEW_KW			:'VIEW';
WITH_KW			:'WITH';
VERSION_KW		:'VERSION';
SETTING_KW		:'SETTING';


/* construction query keywords */
SELECT_KW   :'SELECT';
FROM_KW     :'FROM';
JOIN_KW     :'JOIN';
ON_KW       :'ON';

/* setting keywords */
PROP_UPDATE_SETS_KW	: 'PROPAGATE UPDATE SETS';
PROP_ATTR_DESCR_KW	: 'PROPAGATE ATTRIBUTE DESCRIPTIONS';

/* special operators */
STAR_OP :'*';
EQ_OP	:'==';

/* special chars */
SEMICOLON   :';';
COMMA       :',';
LPAREN      :'(';
RPAREN      :')';
QUOT		:'"';
HASHTAG		:'#';
WHITESPACE	:[ \r\t\n]+ -> skip;

/* literals */
DATETIME	: [0-9][0-9]'-'[0-9][0-9]'-'[0-9][0-9][0-9][0-9]'T'[0-9][0-9]':'[0-9][0-9]':'[0-9][0-9];
INTEGER     :'-'?([1-9]+[0-9]*|[0]);
DECIMAL     :'-'?([1-9]+[0-9]*|[0])('.'[0-9]+);
BOOLEAN     :'TRUE'|'FALSE'|'true'|'false';
STRING      :'"' ('\\"'|.*?) '"';
BINARY      :'0x'([A-F0-9]+|[a-f0-9]+);

/* structure identifiers */
ATTRIBUTE_ID     :[a-zA-Z][a-zA-Z0-9_-]*'.'[a-zA-Z][a-zA-Z0-9_-]*'.'[a-zA-Z][a-zA-Z0-9_-]*'.'[a-zA-Z][a-zA-Z0-9_-]*;
TABLEAU_ID       :[a-zA-Z][a-zA-Z0-9_-]*'.'[a-zA-Z][a-zA-Z0-9_-]*'.'[a-zA-Z][a-zA-Z0-9_-]*;
SCHEMA_ID        :[a-zA-Z][a-zA-Z0-9_-]*'.'[a-zA-Z][a-zA-Z0-9_-]*;
STRUCTURE_NAME	 :[a-zA-Z][a-zA-Z0-9_-]*;

DESCRIPTION_TEXT	:'#' ('\\#'|.*?)? '#';

/* Parser rules */

projection_expr		:ATTRIBUTE_ID (COMMA ATTRIBUTE_ID)*;
version_expr		:VERSION_KW STRING;
description_expr	:DESCRIPTION_TEXT;

/* clauses */
select_clause   :SELECT_KW projection_expr;
from_clause     :FROM_KW TABLEAU_ID (join_clause)*;
join_clause     :JOIN_KW TABLEAU_ID ON_KW ATTRIBUTE_ID EQ_OP ATTRIBUTE_ID;

setting_clause	:SETTING_KW PROP_UPDATE_SETS_KW? PROP_ATTR_DESCR_KW?;

source_query_clause		:select_clause from_clause;
attribute_declaration	:STRUCTURE_NAME description_expr?; 

attribute_mediation		:WITH_KW ATTRIBUTES_KW attribute_declaration (COMMA attribute_declaration)*;

tableau_mediation		:WITH_KW TABLEAU_KW STRUCTURE_NAME description_expr? attribute_mediation BEING_KW source_query_clause;

schema_mediation		:WITH_KW SCHEMA_KW STRUCTURE_NAME description_expr? tableau_mediation*;

datasource_mediation	:(setting_clause)? DATASOURCE_KW STRUCTURE_NAME version_expr? description_expr? schema_mediation*;