using System.Text.RegularExpressions;

namespace Snai.CMS.Api_Core.Common.Infrastructure.Validation
{
    public static class Validator
    {
        private readonly static Regex IPV4Regex = new(@"^((2(5[0-5]|[0-4]\d))|[0-1]?\d{1,2})(\.((2(5[0-5]|[0-4]\d))|[0-1]?\d{1,2})){3}$", RegexOptions.Compiled);
        private readonly static Regex IPV6Regex = new(@"^\s*((([0-9A-Fa-f]{1,4}:){7}([0-9A-Fa-f]{1,4}|:))|(([0-9A-Fa-f]{1,4}:){6}(:[0-9A-Fa-f]{1,4}|((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3})|:))|(([0-9A-Fa-f]{1,4}:){5}(((:[0-9A-Fa-f]{1,4}){1,2})|:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3})|:))|(([0-9A-Fa-f]{1,4}:){4}(((:[0-9A-Fa-f]{1,4}){1,3})|((:[0-9A-Fa-f]{1,4})?:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:))|(([0-9A-Fa-f]{1,4}:){3}(((:[0-9A-Fa-f]{1,4}){1,4})|((:[0-9A-Fa-f]{1,4}){0,2}:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:))|(([0-9A-Fa-f]{1,4}:){2}(((:[0-9A-Fa-f]{1,4}){1,5})|((:[0-9A-Fa-f]{1,4}){0,3}:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:))|(([0-9A-Fa-f]{1,4}:){1}(((:[0-9A-Fa-f]{1,4}){1,6})|((:[0-9A-Fa-f]{1,4}){0,4}:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:))|(:(((:[0-9A-Fa-f]{1,4}){1,7})|((:[0-9A-Fa-f]{1,4}){0,5}:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:)))(%.+)?\s*$", RegexOptions.Compiled);
        private readonly static Regex DomainRegex = new(@"^[a-zA-Z0-9][-a-zA-Z0-9]{0,62}(\.[a-zA-Z0-9][-a-zA-Z0-9]{0,62})+\.?$", RegexOptions.Compiled);
        private readonly static Regex UrlRegex = new(@"^[a-zA-z]+://[^\s]*$", RegexOptions.Compiled);
        private readonly static Regex PhoneNumberRegex = new(@"^(13[0-9]|14[5|7]|15[0|1|2|3|4|5|6|7|8|9]|18[0|1|2|3|5|6|7|8|9])\d{8}$", RegexOptions.Compiled);
        private readonly static Regex EnglishRegex = new(@"^[A-Za-z]+$", RegexOptions.Compiled);
        private readonly static Regex IdentityNumberRegex = new(@"(^\d{15}$)|(^\d{18}$)|(^\d{17}(\d|X|x)$)", RegexOptions.Compiled);
        private readonly static Regex EmailRegex = new(@"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", RegexOptions.Compiled);
        private readonly static Regex ChineseRegex = new(@"^[\u4e00-\u9fa5]{0,}$", RegexOptions.Compiled);
        private readonly static Regex LandlineRegex = new(@"^\d{3}-\d{8}|\d{4}-\d{7}|\d{7}$", RegexOptions.Compiled);
        private readonly static Regex PositiveInteger = new(@"^\d+$", RegexOptions.Compiled);

        /// <summary>
        /// 是否是IPV4格式的IP
        /// </summary>
        /// <returns></returns>
        public static bool IsIPV4(string input)
        {
            return IPV4Regex.IsMatch(input);
        }

        /// <summary>
        /// 是否是IPV6格式的IP
        /// </summary>
        /// <returns></returns>
        public static bool IsIPV6(string input)
        {
            return IPV6Regex.IsMatch(input);
        }

        /// <summary>
        /// 是否是一个域名
        /// </summary>
        /// <returns></returns>
        public static bool IsDomain(string input)
        {
            return DomainRegex.IsMatch(input);
        }

        /// <summary>
        /// 是否是一个网址
        /// </summary>
        /// <returns></returns>
        public static bool IsUrl(string input)
        {
            return UrlRegex.IsMatch(input);
        }

        /// <summary>
        /// 是否是一个手机号码（中国大陆）
        /// </summary>
        /// <returns></returns>
        public static bool IsPhoneNumber(string input)
        {
            return PhoneNumberRegex.IsMatch(input);
        }

        /// <summary>
        /// 是否是纯英文字母
        /// </summary>
        /// <returns></returns>
        public static bool IsEnglish(string input)
        {
            return EnglishRegex.IsMatch(input);
        }

        /// <summary>
        /// 只包含英文字母和数字的组合
        /// </summary>
        /// <returns></returns>
        public static bool IsCombinationOfEnglishNumber(string input, int? minLength = null, int? maxLength = null)
        {
            var pattern = @"(?=.*\d)(?=.*[a-zA-Z])[a-zA-Z0-9]";
            if (minLength is null && maxLength is null)
                pattern = $@"^{pattern}+$";
            else if (minLength is not null && maxLength is null)
                pattern = $@"^{pattern}{{{minLength},}}$";
            else if (minLength is null && maxLength is not null)
                pattern = $@"^{pattern}{{1,{maxLength}}}$";
            else
                pattern = $@"^{pattern}{{{minLength},{maxLength}}}$";
            return Regex.IsMatch(input, pattern);
        }

        /// <summary>
        /// 只包含英文字母、数字和特殊字符的组合
        /// </summary>
        /// <returns></returns>
        public static bool IsCombinationOfEnglishNumberSymbol(string input, int? minLength = null, int? maxLength = null)
        {
            var pattern = @"(?=.*\d)(?=.*[a-zA-Z])(?=.*[^a-zA-Z\d]).";
            if (minLength is null && maxLength is null)
                pattern = $@"^{pattern}+$";
            else if (minLength is not null && maxLength is null)
                pattern = $@"^{pattern}{{{minLength},}}$";
            else if (minLength is null && maxLength is not null)
                pattern = $@"^{pattern}{{1,{maxLength}}}$";
            else
                pattern = $@"^{pattern}{{{minLength},{maxLength}}}$";
            return Regex.IsMatch(input, pattern);
        }

        /// <summary>
        /// 是否是身份证号码（中国大陆）
        /// </summary>
        /// <returns></returns>
        public static bool IsIdentityNumber(string input)
        {
            return IdentityNumberRegex.IsMatch(input);
        }

        /// <summary>
        /// 是否是电子邮箱
        /// </summary>
        /// <returns></returns>
        public static bool IsEmail(string input)
        {
            return EmailRegex.IsMatch(input);
        }

        /// <summary>
        /// 是否是汉字
        /// </summary>
        /// <returns></returns>
        public static bool IsChinese(string input)
        {
            return ChineseRegex.IsMatch(input);
        }

        /// <summary>
        /// 是否是座机号码（中国大陆）
        /// </summary>
        /// <returns></returns>
        public static bool IsLandline(string input)
        {
            return LandlineRegex.IsMatch(input);
        }

        /// <summary>
        /// 是否是正整数
        /// </summary>
        /// <returns></returns>
        public static bool IsPositiveInteger(string input)
        {
            return PositiveInteger.IsMatch(input);
        }
    }
}
