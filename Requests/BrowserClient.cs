using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

// NOTE: This file depends on Microsoft.Playwright. After adding the package run: "playwright install" to download browser binaries.

namespace gc_bot.Requests
{
    /// <summary>
    /// Helper that uses Playwright to open a headless browser, perform login and return final page content and cookies.
    /// This is intended for sites that require JavaScript execution to obtain tokens/redirects.
    /// </summary>
    public static class BrowserClient
    {
        /// <summary>
        /// Launches a headless Chromium, navigates to the loginUrl, fills `user` and `pass` into the provided field selectors,
        /// submits the form (by clicking selectorSubmit) and returns page content and cookies.
        /// </summary>
        /// <remarks>
        /// This implementation is a best-effort example. You may need to adjust selectors to match the site's HTML.
        /// </remarks>
        public static async Task<(string Content, string Cookies)> LoginAndGetContentAsync(string loginUrl, string userSelector, string passSelector, string submitSelector, string user, string pass)
        {
#if USE_PLAYWRIGHT
            using var playwright = await Microsoft.Playwright.Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new Microsoft.Playwright.BrowserTypeLaunchOptions { Headless = true, Args = new[] { "--no-sandbox" } });
            var context = await browser.NewContextAsync();
            var page = await context.NewPageAsync();

            await page.GotoAsync(loginUrl);

            if (!string.IsNullOrWhiteSpace(userSelector)) await page.FillAsync(userSelector, user ?? string.Empty);
            if (!string.IsNullOrWhiteSpace(passSelector)) await page.FillAsync(passSelector, pass ?? string.Empty);

            if (!string.IsNullOrWhiteSpace(submitSelector))
            {
                await page.ClickAsync(submitSelector);
            }
            else
            {
                // try pressing Enter in password field
                if (!string.IsNullOrWhiteSpace(passSelector)) await page.PressAsync(passSelector, "Enter");
            }

            // wait for network idle or navigation
            try { await page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle, new() { Timeout = 5000 }); } catch { }

            var content = await page.ContentAsync();
            var cookies = await context.CookiesAsync();
            var cookiesJson = JsonSerializer.Serialize(cookies);

            await browser.CloseAsync();
            return (content, cookiesJson);
#else
            throw new InvalidOperationException("Playwright integration is disabled. To enable: add Microsoft.Playwright package and define the compilation symbol USE_PLAYWRIGHT. Then run 'playwright install'.");
#endif
        }
    }
}
