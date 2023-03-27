"use strict";
// from https://github.com/ideal-postcodes/postcode
var __assign = (this && this.__assign) || function () {
    __assign = Object.assign || function(t) {
        for (var s, i = 1, n = arguments.length; i < n; i++) {
            s = arguments[i];
            for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p))
                t[p] = s[p];
        }
        return t;
    };
    return __assign.apply(this, arguments);
};
exports.__esModule = true;
exports.fix = exports.FIXABLE_REGEX = exports.replace = exports.match = exports.parse = exports.toSubDistrict = exports.toDistrict = exports.toUnit = exports.toSector = exports.toArea = exports.toIncode = exports.toOutcode = exports.toNormalised = exports.validOutcode = exports.isValid = exports.AREA_REGEX = exports.POSTCODE_CORPUS_REGEX = exports.POSTCODE_REGEX = exports.OUTCODE_REGEX = exports.INCODE_REGEX = exports.UNIT_REGEX = exports.DISTRICT_SPLIT_REGEX = void 0;
/**
 * @hidden
 */
exports.DISTRICT_SPLIT_REGEX = /^([a-z]{1,2}\d)([a-z])$/i;
/**
 * Tests for the unit section of a postcode
 */
exports.UNIT_REGEX = /[a-z]{2}$/i;
/**
 * Tests for the inward code section of a postcode
 */
exports.INCODE_REGEX = /\d[a-z]{2}$/i;
/**
 * Tests for the outward code section of a postcode
 */
exports.OUTCODE_REGEX = /^[a-z]{1,2}\d[a-z\d]?$/i;
/**
 * Tests for a valid postcode
 */
exports.POSTCODE_REGEX = /^[a-z]{1,2}\d[a-z\d]?\s*\d[a-z]{2}$/i;
/**
 * Test for a valid postcode embedded in text
 */
exports.POSTCODE_CORPUS_REGEX = /[a-z]{1,2}\d[a-z\d]?\s*\d[a-z]{2}/gi;
/**
 * Tests for the area section of a postcode
 */
exports.AREA_REGEX = /^[a-z]{1,2}/i;
/**
 * Invalid postcode prototype
 * @hidden
 */
var invalidPostcode = {
    valid: false,
    postcode: null,
    incode: null,
    outcode: null,
    area: null,
    district: null,
    subDistrict: null,
    sector: null,
    unit: null
};
/**
 * Return first elem of input is RegExpMatchArray or null if input null
 * @hidden
 */
var firstOrNull = function (match) {
    if (match === null)
        return null;
    return match[0];
};
var SPACE_REGEX = /\s+/gi;
/**
 * Drop all spaces and uppercase
 * @hidden
 */
var sanitize = function (s) {
    return s.replace(SPACE_REGEX, "").toUpperCase();
};
/**
 * Sanitizes string and returns regex matches
 * @hidden
 */
var matchOn = function (s, regex) {
    return sanitize(s).match(regex);
};
/**
 * Detects a "valid" postcode
 * - Starts and ends on a non-space character
 * - Any length of intervening space is allowed
 * - Must conform to one of following schemas:
 *  - AA1A 1AA
 *  - A1A 1AA
 *  - A1 1AA
 *  - A99 9AA
 *  - AA9 9AA
 *  - AA99 9AA
 */
var isValid = function (postcode) {
    return postcode.match(exports.POSTCODE_REGEX) !== null;
};
exports.isValid = isValid;
/**
 * Returns true if string is a valid outcode
 */
var validOutcode = function (outcode) {
    return outcode.match(exports.OUTCODE_REGEX) !== null;
};
exports.validOutcode = validOutcode;
/**
 * Returns a normalised postcode string (i.e. uppercased and properly spaced)
 *
 * Returns null if invalid postcode
 */
var toNormalised = function (postcode) {
    var outcode = exports.toOutcode(postcode);
    if (outcode === null)
        return null;
    var incode = exports.toIncode(postcode);
    if (incode === null)
        return null;
    return outcode + " " + incode;
};
exports.toNormalised = toNormalised;
/**
 * Returns a correctly formatted outcode given a postcode
 *
 * Returns null if invalid postcode
 */
var toOutcode = function (postcode) {
    if (!exports.isValid(postcode))
        return null;
    return sanitize(postcode).replace(exports.INCODE_REGEX, "");
};
exports.toOutcode = toOutcode;
/**
 * Returns a correctly formatted incode given a postcode
 *
 * Returns null if invalid postcode
 */
var toIncode = function (postcode) {
    if (!exports.isValid(postcode))
        return null;
    var match = matchOn(postcode, exports.INCODE_REGEX);
    return firstOrNull(match);
};
exports.toIncode = toIncode;
/**
 * Returns a correctly formatted area given a postcode
 *
 * Returns null if invalid postcode
 */
var toArea = function (postcode) {
    if (!exports.isValid(postcode))
        return null;
    var match = matchOn(postcode, exports.AREA_REGEX);
    return firstOrNull(match);
};
exports.toArea = toArea;
/**
 * Returns a correctly formatted sector given a postcode
 *
 * Returns null if invalid postcode
 */
var toSector = function (postcode) {
    var outcode = exports.toOutcode(postcode);
    if (outcode === null)
        return null;
    var incode = exports.toIncode(postcode);
    if (incode === null)
        return null;
    return outcode + " " + incode[0];
};
exports.toSector = toSector;
/**
 * Returns a correctly formatted unit given a postcode
 *
 * Returns null if invalid postcode
 */
var toUnit = function (postcode) {
    if (!exports.isValid(postcode))
        return null;
    var match = matchOn(postcode, exports.UNIT_REGEX);
    return firstOrNull(match);
};
exports.toUnit = toUnit;
/**
 * Returns a correctly formatted district given a postcode
 *
 * Returns null if invalid postcode
 *
 * @example
 *
 * ```
 * toDistrict("AA9 9AA") // => "AA9"
 * toDistrict("A9A 9AA") // => "A9"
 * ```
 */
var toDistrict = function (postcode) {
    var outcode = exports.toOutcode(postcode);
    if (outcode === null)
        return null;
    var match = outcode.match(exports.DISTRICT_SPLIT_REGEX);
    if (match === null)
        return outcode;
    return match[1];
};
exports.toDistrict = toDistrict;
/**
 * Returns a correctly formatted subdistrict given a postcode
 *
 * Returns null if no subdistrict is available on valid postcode
 * Returns null if invalid postcode
 *
 * @example
 *
 * ```
 * toSubDistrict("AA9A 9AA") // => "AA9A"
 * toSubDistrict("A9A 9AA") // => "A9A"
 * toSubDistrict("AA9 9AA") // => null
 * toSubDistrict("A9 9AA") // => null
 * ```
 */
var toSubDistrict = function (postcode) {
    var outcode = exports.toOutcode(postcode);
    if (outcode === null)
        return null;
    var split = outcode.match(exports.DISTRICT_SPLIT_REGEX);
    if (split === null)
        return null;
    return outcode;
};
exports.toSubDistrict = toSubDistrict;
/**
 * Returns a ValidPostcode or InvalidPostcode object from a postcode string
 *
 * @example
 *
 * ```
 * import { parse } from "postcode";
 *
 * const {
 * postcode,    // => "SW1A 2AA"
 * outcode,     // => "SW1A"
 * incode,      // => "2AA"
 * area,        // => "SW"
 * district,    // => "SW1"
 * unit,        // => "AA"
 * sector,      // => "SW1A 2"
 * subDistrict, // => "SW1A"
 * valid,       // => true
 * } = parse("Sw1A     2aa");
 *
 * const {
 * postcode,    // => null
 * outcode,     // => null
 * incode,      // => null
 * area,        // => null
 * district,    // => null
 * unit,        // => null
 * sector,      // => null
 * subDistrict, // => null
 * valid,       // => false
 * } = parse("    Oh no, ):   ");
 * ```
 */
var parse = function (postcode) {
    if (!exports.isValid(postcode))
        return __assign({}, invalidPostcode);
    return {
        valid: true,
        postcode: exports.toNormalised(postcode),
        incode: exports.toIncode(postcode),
        outcode: exports.toOutcode(postcode),
        area: exports.toArea(postcode),
        district: exports.toDistrict(postcode),
        subDistrict: exports.toSubDistrict(postcode),
        sector: exports.toSector(postcode),
        unit: exports.toUnit(postcode)
    };
};
exports.parse = parse;
/**
 * Searches a body of text for postcode matches
 *
 * Returns an empty array if no match
 *
 * @example
 *
 * ```
 * // Retrieve valid postcodes in a body of text
 * const matches = match("The PM and her no.2 live at SW1A2aa and SW1A 2AB"); // => ["SW1A2aa", "SW1A 2AB"]
 *
 * // Perform transformations like normalisation postcodes using `.map` and `toNormalised`
 * matches.map(toNormalised); // => ["SW1A 2AA", "SW1A 2AB"]
 *
 * // No matches yields empty array
 * match("Some London outward codes are SW1A, NW1 and E1"); // => []
 * ```
 */
var match = function (corpus) {
    return corpus.match(exports.POSTCODE_CORPUS_REGEX) || [];
};
exports.match = match;
/**
 * Replaces postcodes in a body of text with a string
 *
 * By default the replacement string is empty string `""`
 *
 * @example
 *
 * ```
 * // Replace postcodes in a body of text
 * replace("The PM and her no.2 live at SW1A2AA and SW1A 2AB");
 * // => { match: ["SW1A2AA", "SW1A 2AB"], result: "The PM and her no.2 live at  and " }
 *
 * // Add custom replacement
 * replace("The PM lives at SW1A 2AA", "Downing Street");
 * // => { match: ["SW1A 2AA"], result: "The PM lives at Downing Street" };
 *
 * // No match
 * replace("Some London outward codes are SW1A, NW1 and E1");
 * // => { match: [], result: "Some London outward codes are SW1A, NW1 and E1" }
 * ```
 */
var replace = function (corpus, replaceWith) {
    if (replaceWith === void 0) { replaceWith = ""; }
    return ({
        match: exports.match(corpus),
        result: corpus.replace(exports.POSTCODE_CORPUS_REGEX, replaceWith)
    });
};
exports.replace = replace;
exports.FIXABLE_REGEX = /^\s*[a-z01]{1,2}[0-9oi][a-z\d]?\s*[0-9oi][a-z01]{2}\s*$/i;
/**
 * Attempts to fix and clean a postcode. Specifically:
 * - Performs character conversion on obviously wrong and commonly mixed up letters (e.g. O => 0 and vice versa)
 * - Trims string
 * - Properly adds space between outward and inward codes
 *
 * If the postcode cannot be coerced into a valid format, the original string is returned
 *
 * @example
 * ```javascript
 * fix(" SW1A  2AO") => "SW1A 2AO" // Properly spaces
 * fix("SW1A 2A0") => "SW1A 2AO" // 0 is coerced into "0"
 * ```
 *
 * Aims to be used in conjunction with parse to make postcode entry more forgiving:
 *
 * @example
 * ```javascript
 * const { inward } = parse(fix("SW1A 2A0")); // inward = "2AO"
 * ```
 */
var fix = function (s) {
    var match = s.match(exports.FIXABLE_REGEX);
    if (match === null)
        return s;
    s = s.toUpperCase().trim().replace(/\s+/gi, "");
    var l = s.length;
    var inward = s.slice(l - 3, l);
    return coerceOutcode(s.slice(0, l - 3)) + " " + coerce("NLL", inward);
};
exports.fix = fix;
var toLetter = {
    "0": "O",
    "1": "I"
};
var toNumber = {
    O: "0",
    I: "1"
};
var coerceOutcode = function (i) {
    if (i.length === 2)
        return coerce("LN", i);
    if (i.length === 3)
        return coerce("L??", i);
    if (i.length === 4)
        return coerce("LLN?", i);
    return i;
};
/**
 * Given a pattern of letters, numbers and unknowns represented as a sequence
 * of L, Ns and ? respectively; coerce them into the correct type given a
 * mapping of potentially confused letters
 *
 * @hidden
 *
 * @example coerce("LLN", "0O8") => "OO8"
 */
var coerce = function (pattern, input) {
    return input
        .split("")
        .reduce(function (acc, c, i) {
        var target = pattern.charAt(i);
        if (target === "N")
            acc.push(toNumber[c] || c);
        if (target === "L")
            acc.push(toLetter[c] || c);
        if (target === "?")
            acc.push(c);
        return acc;
    }, [])
        .join("");
};
