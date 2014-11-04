define(function(require, exports, module) {
    "use strict";

    var oop = require("../lib/oop");
    var TextHighlightRules = require("./text_highlight_rules").TextHighlightRules;

    var SparqlHighlightRules = function() {

        var keywords = (
            "ADD|ALL|AS|ASC|ASK|BASE|BIND|BINDINGS|CLEAR|CONSTRUCT|COPY|COUNT|CREATE|" +
            "DATA|DEFAULT|DELETE|DESC|DESCRIBE|DISTINCT|DROP|" +
            "EXISTS|FILTER|FROM|GRAPH|GROUP|BY|HAVING|IF|IN|INSERT|INTO|LIMIT|LOAD|" +
            "MINUS|MODIFY|MOVE|NAMED|NOT|EXISTS|NOT|IN|OFFSET|OPTIONAL|ORDER|BY|" +
            "PREFIX|REDUCED|REPLACE|SELECT|SERVICE|SILENT|TO|UNION|USING|WHERE|WITH"
        );

        var builtinConstants = (
            "true|false|null"
        );

        var builtinFunctions = (
            "abs|AVG|BNODE|bound|ceil|COALESCE|CONCAT|CONTAINS|datatype|day|ENCODE_FOR_URI|" +
            "floor|GROUP_CONCAT|hours|IRI|isBlank|isIRI|isLiteral|isNumeric|isURI|lang|langMatches|" +
            "LCASE|MAX|MD5|MIN|minutes|month|now|rand|regex|round|sameTerm|SAMPLE|seconds|SEPARATOR|SHA1|" +
            "SHA224|SHA256|SHA384|SHA512|str|STRAFTER|STRBEFORE|STRDT|STRENDS|STRLANG|STRLEN|STRSTARTS|" +
            "SUBSTR|SUM|timezone|tz|UCASE|undef|URI|year"
        );

        var keywordMapper = this.createKeywordMapper({
            "support.function": builtinFunctions,
            "keyword": keywords,
            "constant.language": builtinConstants
        }, "identifier", true);

        this.$rules = {
            "start" : [ {
                token : "comment",
                regex : "#.*$"
            }, {
                token : "string",           // " string
                regex : '".*?"'
            }, {
                token : "string",           // ' string
                regex : "'.*?'"
            }, {
                token : "constant.numeric", // float
                regex : "[+-]?\\d+(?:(?:\\.\\d*)?(?:[eE][+-]?\\d+)?)?\\b"
            }, {
                token : keywordMapper,
                regex : "[a-zA-Z_$][a-zA-Z0-9_$]*\\b"
            }, {
                token : "variable",
                regex : "[@?][a-zA-Z_$]+"
            }, {
                token : "keyword.operator",
                regex : "\\+|\\-|\\/|\\/\\/|%|<@>|@>|<@|&|\\^|~|<|>|<=|=>|==|!=|<>|="
            }, {
                token : "paren.lparen",
                regex : "[\\({]"
            }, {
                token : "paren.rparen",
                regex : "[\\)}]"
            }, {
                token : "text",
                regex : "\\s+"
            } ]
        };
        this.normalizeRules();
    };

    oop.inherits(SparqlHighlightRules, TextHighlightRules);

    exports.SparqlHighlightRules = SparqlHighlightRules;
});