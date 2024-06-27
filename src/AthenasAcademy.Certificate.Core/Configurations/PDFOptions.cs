namespace AthenasAcademy.Certificate.Core.Configurations
{
    /// <summary>
    /// Represents options for generating PDF documents.
    /// </summary>
    public class PDFOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PDFOptions"/> class.
        /// </summary>
        public PDFOptions()
        {
            Margin = new PDFMarginOptions();
            Page = new PDFPageOptions();
            Header = new PDFHeaderOptions();
            Footer = new PDFFooterOptions();
        }

        /// <summary>
        /// Gets or sets the title of the PDF document.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Gets or sets the encoding format used for the PDF document.
        /// </summary>
        public string Encoding { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether images are excluded from the PDF.
        /// </summary>
        public bool NoImages { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether JavaScript is disabled in the PDF.
        /// </summary>
        public bool DisableJavascript { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether JavaScript is enabled in the PDF.
        /// </summary>
        public bool EnableJavascript { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the background is excluded from the PDF.
        /// </summary>
        public bool NoBackground { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the PDF is rendered in grayscale.
        /// </summary>
        public bool Grayscale { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether print media type is used in the PDF.
        /// </summary>
        public bool PrintMediaType { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether slow scripts are not stopped in the PDF.
        /// </summary>
        public bool NoStopSlowScripts { get; private set; }

        /// <summary>
        /// Gets or sets the margin settings for the PDF.
        /// </summary>
        public PDFMarginOptions Margin { get; private set; }

        /// <summary>
        /// Gets or sets the page settings for the PDF.
        /// </summary>
        public PDFPageOptions Page { get; private set; }

        /// <summary>
        /// Gets or sets the header settings for the PDF.
        /// </summary>
        public PDFHeaderOptions Header { get; private set; }

        /// <summary>
        /// Gets or sets the footer settings for the PDF.
        /// </summary>
        public PDFFooterOptions Footer { get; private set; }

        /// <summary>
        /// Converts the PDFOptions object to a string of command-line arguments.
        /// </summary>
        /// <returns>A string representing the command-line arguments.</returns>
        public string ToArgumentsString()
        {
            var args = new List<string>();

            if (!string.IsNullOrEmpty(Title)) args.Add($"--title \"{Title}\"");
            if (!string.IsNullOrEmpty(Encoding)) args.Add($"--encoding {Encoding}");
            if (NoImages) args.Add("--no-images");
            if (DisableJavascript) args.Add("--disable-javascript");
            if (EnableJavascript) args.Add("--enable-javascript");
            if (NoBackground) args.Add("--no-background");
            if (Grayscale) args.Add("--grayscale");
            if (PrintMediaType) args.Add("--print-media-type");
            if (NoStopSlowScripts) args.Add("--no-stop-slow-scripts");

            if (Margin.Top != default && Margin.Top >= 1) args.Add($"-T {Margin.Top}");
            if (Margin.Bottom != default && Margin.Bottom >= 1) args.Add($"-B {Margin.Bottom}");
            if (Margin.Left != default && Margin.Left >= 1) args.Add($"-L {Margin.Left}");
            if (Margin.Right != default && Margin.Right >= 1) args.Add($"-R {Margin.Right}");

            if (Page.Size != default && Page.Size >= 1) args.Add($"--page-size {Page.Size}in".Replace(",", "."));
            if (Page.Width != default && Page.Width >= 1) args.Add($"--page-width {Page.Width}in".Replace(",", "."));
            if (Page.Height != default && Page.Height >= 1) args.Add($"--page-height {Page.Height}in".Replace(",", "."));
            if (!string.IsNullOrEmpty(Page.Orientation)) args.Add($"--orientation {Page.Orientation}");
            if (!string.IsNullOrEmpty(Page.Zoom)) args.Add($"--zoom {Page.Zoom}");
            if (!string.IsNullOrEmpty(Page.Dpi)) args.Add($"--dpi {Page.Dpi}");

            if (!string.IsNullOrEmpty(Header.Left)) args.Add($"--header-left \"{Header.Left}\"");
            if (!string.IsNullOrEmpty(Header.Center)) args.Add($"--header-center \"{Header.Center}\"");
            if (!string.IsNullOrEmpty(Header.Right)) args.Add($"--header-right \"{Header.Right}\"");
            if (!string.IsNullOrEmpty(Header.FontSize)) args.Add($"--header-font-size {Header.FontSize}");

            if (!string.IsNullOrEmpty(Footer.Left)) args.Add($"--footer-left \"{Footer.Left}\"");
            if (!string.IsNullOrEmpty(Footer.Center)) args.Add($"--footer-center \"{Footer.Center}\"");
            if (!string.IsNullOrEmpty(Footer.Right)) args.Add($"--footer-right \"{Footer.Right}\"");
            if (!string.IsNullOrEmpty(Footer.FontSize)) args.Add($"--footer-font-size {Footer.FontSize}");
            if (!string.IsNullOrEmpty(Footer.Spacing)) args.Add($"--footer-spacing {Footer.Spacing}");

            return string.Join(" ", args);
        }

        /// <summary>
        /// Builder class for PDFOptions that allows fluent construction of PDFOptions objects.
        /// </summary>
        public class PDFOptionsBuilder
        {
            private readonly PDFOptions _options = new PDFOptions();

            /// <summary>
            /// Sets the title of the PDF document.
            /// </summary>
            /// <param name="title">The title to set.</param>
            /// <returns>The PDFOptionsBuilder instance for method chaining.</returns>
            public PDFOptionsBuilder SetTitle(string title)
            {
                _options.Title = title;
                return this;
            }

            /// <summary>
            /// Sets the encoding format used for the PDF document.
            /// </summary>
            /// <param name="encoding">The encoding to set.</param>
            /// <returns>The PDFOptionsBuilder instance for method chaining.</returns>
            public PDFOptionsBuilder SetEncoding(string encoding)
            {
                _options.Encoding = encoding;
                return this;
            }

            /// <summary>
            /// Sets whether images should be excluded from the PDF.
            /// </summary>
            /// <param name="noImages">Boolean indicating whether to exclude images.</param>
            /// <returns>The PDFOptionsBuilder instance for method chaining.</returns>
            public PDFOptionsBuilder SetNoImages(bool noImages)
            {
                _options.NoImages = noImages;
                return this;
            }

            /// <summary>
            /// Sets whether JavaScript should be disabled in the PDF.
            /// </summary>
            /// <param name="disableJavascript">Boolean indicating whether to disable JavaScript.</param>
            /// <returns>The PDFOptionsBuilder instance for method chaining.</returns>
            public PDFOptionsBuilder SetDisableJavascript(bool disableJavascript)
            {
                _options.DisableJavascript = disableJavascript;
                return this;
            }

            /// <summary>
            /// Sets whether JavaScript should be enabled in the PDF.
            /// </summary>
            /// <param name="enableJavascript">Boolean indicating whether to enable JavaScript.</param>
            /// <returns>The PDFOptionsBuilder instance for method chaining.</returns>
            public PDFOptionsBuilder SetEnableJavascript(bool enableJavascript)
            {
                _options.EnableJavascript = enableJavascript;
                return this;
            }

            /// <summary>
            /// Sets whether the background should be excluded from the PDF.
            /// </summary>
            /// <param name="noBackground">Boolean indicating whether to exclude the background.</param>
            /// <returns>The PDFOptionsBuilder instance for method chaining.</returns>
            public PDFOptionsBuilder SetNoBackground(bool noBackground)
            {
                _options.NoBackground = noBackground;
                return this;
            }

            /// <summary>
            /// Sets whether the PDF should be rendered in grayscale.
            /// </summary>
            /// <param name="grayscale">Boolean indicating whether to render in grayscale.</param>
            /// <returns>The PDFOptionsBuilder instance for method chaining.</returns>
            public PDFOptionsBuilder SetGrayscale(bool grayscale)
            {
                _options.Grayscale = grayscale;
                return this;
            }

            /// <summary>
            /// Sets whether to use print media type in the PDF.
            /// </summary>
            /// <param name="printMediaType">Boolean indicating whether to use print media type.</param>
            /// <returns>The PDFOptionsBuilder instance for method chaining.</returns>
            public PDFOptionsBuilder SetPrintMediaType(bool printMediaType)
            {
                _options.PrintMediaType = printMediaType;
                return this;
            }

            /// <summary>
            /// Sets whether to not stop slow scripts in the PDF.
            /// </summary>
            /// <param name="noStopSlowScripts">Boolean indicating whether to not stop slow scripts.</param>
            /// <returns>The PDFOptionsBuilder instance for method chaining.</returns>
            public PDFOptionsBuilder SetNoStopSlowScripts(bool noStopSlowScripts)
            {
                _options.NoStopSlowScripts = noStopSlowScripts;
                return this;
            }

            /// <summary>
            /// Sets the margins of the PDF.
            /// </summary>
            /// <param name="top">Top margin.</param>
            /// <param name="bottom">Bottom margin.</param```csharp
            /// <param name="left">Left margin.</param>
            /// <param name="right">Right margin.</param>
            /// <returns>The PDFOptionsBuilder instance for method chaining.</returns>
            public PDFOptionsBuilder SetMargins(int top, int bottom, int left, int right)
            {
                _options.Margin = new PDFMarginOptions { Top = top, Bottom = bottom, Left = left, Right = right };
                return this;
            }

            /// <summary>
            /// Sets the page size of the PDF.
            /// </summary>
            /// <param name="size">The size of the page.</param>
            /// <returns>The PDFOptionsBuilder instance for method chaining.</returns>
            public PDFOptionsBuilder SetPageSize(int size)
            {
                _options.Page.Size = size;
                return this;
            }

            /// <summary>
            /// Sets the width of the PDF page.
            /// </summary>
            /// <param name="width">The width of the page.</param>
            /// <returns>The PDFOptionsBuilder instance for method chaining.</returns>
            public PDFOptionsBuilder SetPageWidth(double width)
            {
                _options.Page.Width = width;
                return this;
            }

            /// <summary>
            /// Sets the height of the PDF page.
            /// </summary>
            /// <param name="height">The height of the page.</param>
            /// <returns>The PDFOptionsBuilder instance for method chaining.</returns>
            public PDFOptionsBuilder SetPageHeight(double height)
            {
                _options.Page.Height = height;
                return this;
            }

            /// <summary>
            /// Sets the orientation of the PDF pages.
            /// </summary>
            /// <param name="orientation">The orientation of the pages (portrait or landscape).</param>
            /// <returns>The PDFOptionsBuilder instance for method chaining.</returns>
            public PDFOptionsBuilder SetOrientation(string orientation)
            {
                _options.Page.Orientation = orientation;
                return this;
            }

            /// <summary>
            /// Sets the zoom level of the PDF pages.
            /// </summary>
            /// <param name="zoom">The zoom level of the pages.</param>
            /// <returns>The PDFOptionsBuilder instance for method chaining.</returns>
            public PDFOptionsBuilder SetZoom(string zoom)
            {
                _options.Page.Zoom = zoom;
                return this;
            }

            /// <summary>
            /// Sets the DPI (dots per inch) of the PDF pages.
            /// </summary>
            /// <param name="dpi">The DPI of the pages.</param>
            /// <returns>The PDFOptionsBuilder instance for method chaining.</returns>
            public PDFOptionsBuilder SetDpi(string dpi)
            {
                _options.Page.Dpi = dpi;
                return this;
            }

            /// <summary>
            /// Sets the header of the PDF.
            /// </summary>
            /// <param name="left">Text on the left side of the header.</param>
            /// <param name="center">Text in the center of the header.</param>
            /// <param name="right">Text on the right side of the header.</param>
            /// <param name="fontSize">Font size of the header text.</param>
            /// <returns>The PDFOptionsBuilder instance for method chaining.</returns>
            public PDFOptionsBuilder SetHeader(string left, string center, string right, string fontSize)
            {
                _options.Header = new PDFHeaderOptions { Left = left, Center = center, Right = right, FontSize = fontSize };
                return this;
            }

            /// <summary>
            /// Sets the footer of the PDF.
            /// </summary>
            /// <param name="left">Text on the left side of the footer.</param>
            /// <param name="center">Text in the center of the footer.</param>
            /// <param name="right">Text on the right side of the footer.</param>
            /// <param name="fontSize">Font size of the footer text.</param>
            /// <param name="spacing">Spacing of the footer from the bottom of the page.</param>
            /// <returns>The PDFOptionsBuilder instance for method chaining.</returns>
            public PDFOptionsBuilder SetFooter(string left, string center, string right, string fontSize, string spacing)
            {
                _options.Footer = new PDFFooterOptions { Left = left, Center = center, Right = right, FontSize = fontSize, Spacing = spacing };
                return this;
            }

            /// <summary>
            /// Builds and returns the configured PDFOptions instance.
            /// </summary>
            /// <returns>The configured PDFOptions instance.</returns>
            public PDFOptions Build()
            {
                return _options;
            }
        }
    }

    /// <summary>
    /// Represents footer options for the PDF.
    /// </summary>
    public class PDFFooterOptions
    {
        /// <summary>
        /// Gets or sets the text on the left side of the footer.
        /// </summary>
        public string Left { get; set; }

        /// <summary>
        /// Gets or sets the text in the center of the footer.
        /// </summary>
        public string Center { get; set; }

        /// <summary>
        /// Gets or sets the text on the right side of the footer.
        /// </summary>
        public string Right { get; set; }

        /// <summary>
        /// Gets or sets the font size of the footer text.
        /// </summary>
        public string FontSize { get; set; }

        /// <summary>
        /// Gets or sets the spacing of the footer from the bottom of the page.
        /// </summary>
        public string Spacing { get; set; }
    }

    /// <summary>
    /// Represents header options for the PDF.
    /// </summary>
    public class PDFHeaderOptions
    {
        /// <summary>
        /// Gets or sets the text on the left side of the header.
        /// </summary>
        public string Left { get; set; }

        /// <summary>
        /// Gets or sets the text in the center of the header.
        /// </summary>
        public string Center { get; set; }

        /// <summary>
        /// Gets or sets the text on the right side of the header.
        /// </summary>
        public string Right { get; set; }

        /// <summary>
        /// Gets or sets the font size of the header text.
        /// </summary>
        public string FontSize { get; set; }
    }

    /// <summary>
    /// Represents page options for the PDF.
    /// </summary>
    public class PDFPageOptions
    {
        /// <summary>
        /// Gets or sets the size of the page.
        /// </summary>
        public double Size { get; set; }

        /// <summary>
        /// Gets or sets the width of the page.
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// Gets or sets the height of the page.
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// Gets or sets the orientation of the page (Portrait or Landscape).
        /// </summary>
        public string Orientation { get; set; }

        /// <summary>
        /// Gets or sets the zoom level of the page.
        /// </summary>
        public string Zoom { get; set; }

        /// <summary>
        /// Gets or sets the DPI (dots per inch) of the page.
        /// </summary>
        public string Dpi { get; set; }
    }

    /// <summary>
    /// Represents margin options for the PDF.
    /// </summary>
    public class PDFMarginOptions
    {
        /// <summary>
        /// Gets or sets the top margin of the page.
        /// </summary>
        public int Top { get; set; }

        /// <summary>
        /// Gets or sets the bottom margin of the page.
        /// </summary>
        public int Bottom { get; set; }

        /// <summary>
        /// Gets or sets the left margin of the page.
        /// </summary>
        public int Left { get; set; }

        /// <summary>
        /// Gets or sets the right margin of the page.
        /// </summary>
        public int Right { get; set; }
    }
}
