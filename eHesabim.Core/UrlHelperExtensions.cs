using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eHesabim.Core {
    public static class UrlHelperExtensions {
        public static string SetUrl(string url, string paramters) {
            return string.Format("{0}{1}", url, paramters);
        }

        public static string SetParameter(string url, string key, string value) {
            return SetParameters(url, new Dictionary<string, object> { { key, value } });
        }

        public static string SetParameters(string url, IEnumerable<KeyValuePair<string, object>> parameters) {
            // var facetRegex = new Regex("^f_", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var parts = url.Split('?');

            IDictionary<string, string> qs = new Dictionary<string, string>();
            if (parts.Length > 1) {
                qs = ParseQueryString(parts[1]);
            }

            foreach (var p in parameters) {
                // merge only if facet fields
                ////if (facetRegex.IsMatch(p.Key)) {
                ////    if (qs.ContainsKey(p.Key)) {
                ////        if (!qs[p.Key].Contains(p.Value + "-")) {
                ////            qs[p.Key] += p.Value.ToNullOrString() + "-";
                ////        }
                ////    }
                ////    else {
                ////        qs[p.Key] = p.Value.ToNullOrString() + "-";
                ////    }
                ////}
                ////else {
                    qs[p.Key] = p.Value.ToNullOrString();
                ////}
            }

            return parts[0] + "?" + DictToQuerystring(qs);
        }

        public static string SetParameters(IDictionary<string, string> qs, IEnumerable<KeyValuePair<string, object>> parameters) {
            ////var facetRegex = new Regex("^f_", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            foreach (var p in parameters.Where(a => a.Value != null)) {
                //// merge only if facet fields
                ////if (facetRegex.IsMatch(p.Key)) {
                ////    if (qs.ContainsKey(p.Key)) {
                ////        if (!qs[p.Key].Contains(p.Value + "-")) {
                ////            qs[p.Key] += p.Value.ToNullOrString() + "-";
                ////        }
                ////    }
                ////    else {
                ////        qs[p.Key] = p.Value.ToNullOrString() + "-";
                ////    }
                ////}
                ////else {
                    qs[p.Key] = p.Value.ToNullOrString();
                ////}
            }

            return DictToQuerystring(qs);
        }

        public static string RemoveParametersUrl(string url, params string[] parameters) {
            var parts = url.Split('?');

            IDictionary<string, string> qs = new Dictionary<string, string>();
            if (parts.Length > 1) {
                qs = ParseQueryString(parts[1]);
            }

            foreach (var p in parameters) {
                qs.Remove(p);
            }

            return parts[0] + "?" + DictToQuerystring(qs);
        }

        public static IDictionary<string, string> RemoveParametersUrl(IDictionary<string, string> qs, string parameter, string value) {
            var qp = qs.Where(q => q.Key == parameter).ToList();
            foreach (var q in qp.Where(q => q.Value.Split('-').Contains(value))) {
                qs[q.Key] = q.Value.Replace(value + "-", string.Empty).Trim();
                if (qs[q.Key] == string.Empty) {
                    qs.Remove(q.Key);
                }
            }

            return qs;
        }

        public static string RemoveParametersUrl(string url, string parameter, string value) {
            var parts = url.Split('?');

            IDictionary<string, string> qs = new Dictionary<string, string>();
            if (parts.Length > 1) {
                qs = ParseQueryString(parts[1]);
            }

            var qp = qs.Where(q => q.Key == parameter).ToList();
            foreach (var q in qp.Where(q => q.Value.Split('-').Contains(value))) {
                qs[q.Key] = q.Value.Replace(value + "-", string.Empty).Trim();
                if (qs[q.Key] == string.Empty) {
                    qs.Remove(q.Key);
                }
            }

            return parts[0] + "?" + DictToQuerystring(qs);
        }

        public static string RemoveParameters(this UrlHelper helper, params string[] parameters) {
            return RemoveParametersUrl(helper.RequestContext.HttpContext.Request.RawUrl, parameters);
        }

        public static string DictToQuerystring(IEnumerable<KeyValuePair<string, string>> qs) {
            return string.Join("&", qs.Where(k => !string.IsNullOrEmpty(k.Key)).Select(k => string.Format("{0}={1}", HttpUtility.UrlEncode(k.Key), HttpUtility.UrlEncode(k.Value))).ToArray());
        }

        public static string SetParameter(this UrlHelper helper, string key, object value) {
            return SetParameter(helper.RequestContext.HttpContext.Request.RawUrl, key, value.ToNullOrString());
        }

        public static string SetParameters(this UrlHelper helper, object parameterDictionary) {
            return SetParameters(helper.RequestContext.HttpContext.Request.RawUrl, (IDictionary<string, object>)parameterDictionary);
        }

        public static IDictionary<string, string> ParseQueryString(string queryString) {
            var qs = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            if (queryString == null) {
                return qs;
            }

            var parts = queryString.Split('?');
            if (parts.Length > 1) {
                queryString = parts[1];
            }

            foreach (var kv in queryString.Split('&').Where(kv => kv.Split('=').Length > 1 && !string.IsNullOrEmpty(kv.Split('=')[1]))) {
                qs[HttpUtility.UrlDecode(kv.Split('=')[0])] = HttpUtility.UrlDecode(kv.Split('=')[1]);
            }

            return qs;
        }

        public static string ForQuery(this UrlHelper helper, string solrQuery) {
            return helper.SetParameter("q", solrQuery);
        }

        public static string RemoveKeyFromQueryString(string url, IEnumerable<string> keys) {
            var parts = url.Split('?');
            IDictionary<string, string> qs = new Dictionary<string, string>();
            if (parts.Length > 1) {
                qs = ParseQueryString(parts[1]);
            }

            foreach (var key in keys) {
                var localkey = "f_" + key;

                if (qs.ContainsKey(localkey)) {
                    qs.Remove(localkey);
                }
            }

            return parts[0] + "?" + DictToQuerystring(qs);
        }

        public static Dictionary<string, string> RemoveKeyFromQueryString(Dictionary<string, string> qs, IEnumerable<string> keys) {
            foreach (var key in keys) {
                var localkey = "f_" + key;

                if (qs.ContainsKey(localkey)) {
                    qs.Remove(localkey);
                }
            }

            return qs;
        }

        public static string RemoveFacets(this UrlHelper helper) {
            var url = HttpUtility.UrlDecode(helper.RequestContext.HttpContext.Request.RawUrl);
            var parts = url.Split('?');

            IDictionary<string, string> qs = new Dictionary<string, string>();
            if (parts.Length > 1) {
                qs = ParseQueryString(parts[1]);
            }

            var qp = qs.Where(q => q.Key != "f_c1" && q.Key != "f_c2").ToList();
            foreach (var q in qp) {
                qs.Remove(q.Key);
            }

            return qs.ContainsKey("f_c1") || qs.ContainsKey("f_c2") ? string.Format("{0}?{1}", parts[0], DictToQuerystring(qs)) : parts[0];
        }

        public static string RemoveQuery(this UrlHelper helper) {
            var url = HttpUtility.UrlDecode(helper.RequestContext.HttpContext.Request.RawUrl);
            var parts = url.Split('?');

            IDictionary<string, string> qs = new Dictionary<string, string>();
            if (parts.Length > 1) {
                qs = ParseQueryString(parts[1]);
            }

            if (qs.ContainsKey("q")) {
                qs.Remove("q");
            }

            return qs.Any() ? string.Format("{0}?{1}", parts[0], DictToQuerystring(qs)) : parts[0];
        }

        public static string ToNullOrString(this object o) {
            return o == null ? null : o.ToString();
        }

        public static bool CheckParameter(this UrlHelper helper, string key, string value, bool initialValue = false) {
            var url = helper.RequestContext.HttpContext.Request.RawUrl;
            var parts = url.Split('?');

            IDictionary<string, string> qs = new Dictionary<string, string>();
            if (parts.Length > 1) {
                qs = ParseQueryString(parts[1]);
            }

            return qs.ContainsKey(key) ? qs.Any(a => a.Key == key && a.Value == value) : initialValue;
        }
        
        public static int GetPathNumber(this UrlHelper helper) {
            var url = helper.RequestContext.HttpContext.Request.RawUrl;

            var parts = url.Split('?');
            var urlArr = parts[0].Split('/');
            var no = urlArr[urlArr.Count() - 1];

            int outNo;
            int.TryParse(no, out outNo);
            return outNo;
        }
    }
}
