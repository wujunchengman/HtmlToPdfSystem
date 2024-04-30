using Microsoft.Playwright;

namespace HtmlToPdfApi
{
    /// <summary>
    /// 转换PDF的Html模板
    /// </summary>
    /// <param name="Html">内容模板</param>
    /// <param name="Header">页眉</param>
    /// <param name="Footer">页脚</param>
    public record ConvertDto(string Html, string? Header, string? Footer);

    public record ConvertV2Dto(string Html, PagePdfOptions? PagePdfOptions = null);
}