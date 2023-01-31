grammar CommandLanguage;
/* LEXER RULES */

/* keywords */
DELETE_KW   :'DELETE';
INSERT_KW   :'INSERT';
UPDATE_KW   :'UPDATE';
FROM_KW     :'FROM';
WHERE_KW    :'WHERE';
INTO_KW     :'INTO';
VALUES_KW   :'VALUES';
WITH_KW     :'WITH';

/* operators */
/* logical operators */
AND_OP  :'AND';
OR_OP   :'OR';
NOT_OP  :'NOT';

/* comparison operators */
GTE_OP  :'>=';
GT_OP   :'>';
LTE_OP  :'<=';
LT_OP   :'<';
EQ_OP   :'==';
NEQ_OP  :'!=';

/* special operators */
STAR_OP     :'*';
MUTATION_OP   :'<-';

/* special chars */
SEMICOLON   :';';
COMMA       :',';
LPAREN      :'(';
RPAREN      :')';
WHITESPACE	:[ \r\t\n]+ -> skip;

/* literals */
DATETIME	: [0-9][0-9]'-'[0-9][0-9]'-'[0-9][0-9][0-9][0-9]('T'[0-9][0-9]':'[0-9][0-9]':'[0-9][0-9])?;
LONGINT     :'-'?([1-9]+[0-9]*|[0])'L';
INTEGER     :'-'?([1-9]+[0-9]*|[0]);
DECIMAL     :'-'?([1-9]+[0-9]*|[0])('.'[0-9]+);
BOOLEAN     :'TRUE'|'FALSE'|'true'|'false';
STRING      :'"' ('\\""'|.*?) '"';
BINARY      :'0x'([A-F0-9]+|[a-f0-9]+);

/* structure identifiers */
ATTRIBUTE_ID     :[a-zA-Z][a-zA-Z0-9_-]*'.'[a-zA-Z][a-zA-Z0-9_-]*'.'[a-zA-Z][a-zA-Z0-9_-]*'.'[a-zA-Z][a-zA-Z0-9_-]*;
TABLEAU_ID       :[a-zA-Z][a-zA-Z0-9_-]*'.'[a-zA-Z][a-zA-Z0-9_-]*'.'[a-zA-Z][a-zA-Z0-9_-]*;
SCHEMA_ID        :[a-zA-Z][a-zA-Z0-9_-]*'.'[a-zA-Z][a-zA-Z0-9_-]*;
STRUCTURE_NAME    :[a-zA-Z][a-zA-Z0-9_-]*;

/* Parser rules */
/* literals */
literal :STRING|DATETIME|INTEGER|DECIMAL|BOOLEAN|BINARY|LONGINT;

/* misc expressions */
mutation_expr               :STRUCTURE_NAME MUTATION_OP literal;
attribute_name_list_expr    :LPAREN STRUCTURE_NAME (COMMA STRUCTURE_NAME)* RPAREN;
values_tuple_expr           :LPAREN literal (COMMA literal)* RPAREN;

/* comparison expressions */
lvalue          :ATTRIBUTE_ID;
rvalue          :literal;
gte_expr        :lvalue GTE_OP rvalue;
gt_expr         :lvalue GT_OP rvalue;
lte_expr        :lvalue LTE_OP rvalue;
lt_expr         :lvalue LT_OP rvalue;
eq_expr         :lvalue EQ_OP rvalue;
neq_expr        :lvalue NEQ_OP rvalue;
comparison_expr 
    : gt_expr
    | gte_expr
    | lte_expr
    | lt_expr
    | eq_expr
    | neq_expr
    ;

/* selection expressions */
/* left recursion can be maintained within just one projection, that's why there are no and_expr, or_expr, not_expr */
selection_expr
    : BOOLEAN
    | comparison_expr
    | NOT_OP LPAREN selection_expr RPAREN
    | selection_expr AND_OP selection_expr
    | selection_expr OR_OP  selection_expr
    | LPAREN selection_expr RPAREN
    ;


/* clauses */
where_clause            :WHERE_KW selection_expr;
from_clause             :FROM_KW TABLEAU_ID;
into_clause             :INTO_KW TABLEAU_ID attribute_name_list_expr;
instantiation_clause    :VALUES_KW (values_tuple_expr)+;
mutation_clause         :WITH_KW mutation_expr (COMMA mutation_expr)*;


/* query */
delete_command  :DELETE_KW from_clause where_clause;
insert_command  :INSERT_KW into_clause instantiation_clause;
update_command  :UPDATE_KW TABLEAU_ID mutation_clause where_clause;

command :delete_command|insert_command|update_command SEMICOLON;


