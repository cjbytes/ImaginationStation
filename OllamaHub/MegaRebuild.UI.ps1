param(
    [string]$Base = "C:\Users\Cody\source\repos\OllamaHub"
)

$ErrorActionPreference = "Stop"

function Write-Step($msg) { Write-Host "`n>>> $msg" -ForegroundColor Cyan }
function Write-Ok($msg)   { Write-Host "OK: $msg" -ForegroundColor Green }

$root   = Join-Path $Base "OllamaHub"
$views  = Join-Path $root "Views"
$styles = Join-Path $root "Styles"

Write-Step "Ensuring UI folders exist"
@($views, $styles) | ForEach-Object {
    if (-not (Test-Path $_)) { New-Item -ItemType Directory -Force -Path $_ | Out-Null }
}
Write-Ok "UI folders ready"

# THEME / COLORS / SHARED RESOURCES
Write-Step "Writing Styles\AppTheme.xaml"
@"
<ResourceDictionary
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">

    <!-- Base palettes -->
    <SolidColorBrush x:Key=""AppBackgroundDarkBrush"" Color=""#1E1E2E""/>
    <SolidColorBrush x:Key=""AppBackgroundLightBrush"" Color=""#F5F5FA""/>

    <!-- Accent options (you can bind to these later via settings) -->
    <SolidColorBrush x:Key=""AccentPurpleBrush"" Color=""#7C6AF7""/>
    <SolidColorBrush x:Key=""AccentBlueBrush""   Color=""#60A5FA""/>
    <SolidColorBrush x:Key=""AccentTealBrush""   Color=""#2DD4BF""/>
    <SolidColorBrush x:Key=""AccentGreenBrush""  Color=""#4ADE80""/>

    <!-- Surfaces -->
    <SolidColorBrush x:Key=""SurfaceLevel1Brush"" Color=""#252535""/>
    <SolidColorBrush x:Key=""SurfaceLevel2Brush"" Color=""#2D2D42""/>
    <SolidColorBrush x:Key=""SurfaceLevelLightBrush"" Color=""#FFFFFF""/>
    <SolidColorBrush x:Key=""BorderBrushSoft"" Color=""#383850""/>

    <!-- Chat bubbles -->
    <Style x:Key=""UserBubbleStyle"" TargetType=""Border"">
        <Setter Property=""CornerRadius"" Value=""14""/>
        <Setter Property=""Padding"" Value=""10,6""/>
        <Setter Property=""Margin"" Value=""8,4""/>
        <Setter Property=""Background"" Value=""{StaticResource AccentBlueBrush}""/>
    </Style>

    <Style x:Key=""AssistantBubbleStyle"" TargetType=""Border"">
        <Setter Property=""CornerRadius"" Value=""14""/>
        <Setter Property=""Padding"" Value=""10,6""/>
        <Setter Property=""Margin"" Value=""8,4""/>
        <Setter Property=""Background"" Value=""{StaticResource SurfaceLevel2Brush}""/>
    </Style>

    <!-- Chat text -->
    <Style x:Key=""UserBubbleTextStyle"" TargetType=""TextBlock"">
        <Setter Property=""Foreground"" Value=""White""/>
        <Setter Property=""TextWrapping"" Value=""Wrap""/>
        <Setter Property=""FontSize"" Value=""14""/>
    </Style>

    <Style x:Key=""AssistantBubbleTextStyle"" TargetType=""TextBlock"">
        <Setter Property=""Foreground"" Value=""#E5E7EB""/>
        <Setter Property=""TextWrapping"" Value=""Wrap""/>
        <Setter Property=""FontSize"" Value=""14""/>
    </Style>

</ResourceDictionary>
"@ | Set-Content -Path (Join-Path $styles "AppTheme.xaml") -Encoding UTF8
Write-Ok "Styles\AppTheme.xaml written"

# CHAT PAGE
Write-Step "Writing Views\ChatPage.xaml"
@"
<Page
    x:Class=""OllamaHub.Views.ChatPage""
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    mc:Ignorable=""d"">

    <Page.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary Source=""ms-appx:///Styles/AppTheme.xaml""/>
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Page.Resources>

    <Grid Background=""{StaticResource AppBackgroundDarkBrush}"">
        <Grid.RowDefinitions>
            <!-- Header / toolbar -->
            <RowDefinition Height=""Auto""/>
            <!-- Main body -->
            <RowDefinition Height=""*""/>
            <!-- Input row -->
            <RowDefinition Height=""Auto""/>
        </Grid.RowDefinitions>

        <!-- HEADER -->
        <Grid Grid.Row=""0"" Background=""{StaticResource SurfaceLevel2Brush}"" Padding=""12,8"">
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width=""*""/>
                <ColumnDefinition Width=""Auto""/>
            </Grid.ColumnDefinitions>

            <StackPanel Orientation=""Horizontal"" Spacing=""8"">
                <TextBlock Text=""Chat Workspace"" FontSize=""18"" FontWeight=""SemiBold"" Foreground=""White""/>
                <TextBlock Text=""·"" Margin=""4,0"" Foreground=""#9CA3AF""/>
                <TextBlock Text=""Project-aware, customizable chat"" Foreground=""#9CA3AF""/>
            </StackPanel>

            <StackPanel Grid.Column=""1"" Orientation=""Horizontal"" Spacing=""8"">
                <ComboBox Width=""180"" PlaceholderText=""Model / Profile""/>
                <ComboBox Width=""140"" PlaceholderText=""Persona""/>
                <Button Content=""⚙"" Width=""32"" Height=""32""/>
            </StackPanel>
        </Grid>

        <!-- MAIN BODY: LEFT CONTEXT + RIGHT CHAT -->
        <Grid Grid.Row=""1"">
            <Grid.ColumnDefinitions>
                <!-- Left context panel -->
                <ColumnDefinition Width=""320"" MinWidth=""220""/>
                <!-- Splitter -->
                <ColumnDefinition Width=""5""/>
                <!-- Chat area -->
                <ColumnDefinition Width=""*""/>
            </Grid.ColumnDefinitions>

            <!-- LEFT CONTEXT PANEL -->
            <Border Grid.Column=""0"" Background=""{StaticResource SurfaceLevel1Brush}"" BorderBrush=""{StaticResource BorderBrushSoft}"" BorderThickness=""0,0,1,0"">
                <Grid Padding=""10"">
                    <Grid.RowDefinitions>
                        <RowDefinition Height=""Auto""/>
                        <RowDefinition Height=""*""/>
                        <RowDefinition Height=""Auto""/>
                    </Grid.RowDefinitions>

                    <TextBlock Text=""Context Panel"" FontWeight=""SemiBold"" Foreground=""White"" Margin=""0,0,0,8""/>

                    <ScrollViewer Grid.Row=""1"">
                        <StackPanel Spacing=""8"">
                            <TextBlock Text=""Active Project"" Foreground=""#9CA3AF""/>
                            <Border Background=""{StaticResource SurfaceLevel2Brush}"" Padding=""8"" CornerRadius=""8"">
                                <TextBlock Text=""(Bind: project name, repo path, etc.)"" Foreground=""#E5E7EB"" TextWrapping=""Wrap""/>
                            </Border>

                            <TextBlock Text=""Attached Files / Docs"" Foreground=""#9CA3AF"" Margin=""0,8,0,0""/>
                            <Border Background=""{StaticResource SurfaceLevel2Brush}"" Padding=""8"" CornerRadius=""8"">
                                <TextBlock Text=""(List of files, specs, logs, etc.)"" Foreground=""#E5E7EB"" TextWrapping=""Wrap""/>
                            </Border>

                            <TextBlock Text=""Quick Actions"" Foreground=""#9CA3AF"" Margin=""0,8,0,0""/>
                            <WrapPanel ItemHeight=""32"" ItemWidth=""120"" Orientation=""Horizontal"">
                                <Button Content=""Explain file"" Margin=""0,0,8,8""/>
                                <Button Content=""Refactor"" Margin=""0,0,8,8""/>
                                <Button Content=""Generate tests"" Margin=""0,0,8,8""/>
                                <Button Content=""Summarize"" Margin=""0,0,8,8""/>
                            </WrapPanel>
                        </StackPanel>
                    </ScrollViewer>

                    <!-- Context footer -->
                    <StackPanel Grid.Row=""2"" Orientation=""Horizontal"" Spacing=""8"" Margin=""0,8,0,0"">
                        <Button Content=""Attach…"" HorizontalAlignment=""Left""/>
                        <Button Content=""Clear context""/>
                    </StackPanel>
                </Grid>
            </Border>

            <!-- SPLITTER -->
            <GridSplitter Grid.Column=""1"" HorizontalAlignment=""Stretch"" VerticalAlignment=""Stretch"" Background=""{StaticResource BorderBrushSoft}"" ResizeBehavior=""PreviousAndNext"" ResizeDirection=""Columns""/>

            <!-- CHAT AREA -->
            <Grid Grid.Column=""2"">
                <Grid.RowDefinitions>
                    <RowDefinition Height=""*""/>
                </Grid.RowDefinitions>

                <ScrollViewer VerticalScrollBarVisibility=""Auto"">
                    <ItemsControl>
                        <ItemsControl.Resources>
                            <Style TargetType=""StackPanel"">
                                <Setter Property=""Margin"" Value=""0,4""/>
                            </Style>
                        </ItemsControl.Resources>

                        <!-- SAMPLE USER MESSAGE -->
                        <StackPanel HorizontalAlignment=""Right"">
                            <Border Style=""{StaticResource UserBubbleStyle}"">
                                <TextBlock Style=""{StaticResource UserBubbleTextStyle}""
                                           Text=""This is a sample user message bubble. It should be large, readable, and wrap nicely."" />
                            </Border>
                        </StackPanel>

                        <!-- SAMPLE ASSISTANT MESSAGE -->
                        <StackPanel HorizontalAlignment=""Left"">
                            <Border Style=""{StaticResource AssistantBubbleStyle}"">
                                <TextBlock Style=""{StaticResource AssistantBubbleTextStyle}""
                                           Text=""This is a sample assistant response bubble. Later this will be bound to your chat messages collection."" />
                            </Border>
                        </StackPanel>

                    </ItemsControl>
                </ScrollViewer>
            </Grid>
        </Grid>

        <!-- INPUT ROW -->
        <Border Grid.Row=""2"" Background=""{StaticResource SurfaceLevel2Brush}"" Padding=""10,8"">
            <Grid>
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width=""*""/>
                    <ColumnDefinition Width=""Auto""/>
                </Grid.ColumnDefinitions>

                <Grid Grid.Column=""0"">
                    <Grid.RowDefinitions>
                        <RowDefinition Height=""*""/>
                        <RowDefinition Height=""Auto""/>
                    </Grid.RowDefinitions>

                    <TextBox
                        Grid.Row=""0""
                        AcceptsReturn=""True""
                        TextWrapping=""Wrap""
                        MinHeight=""60""
                        MaxHeight=""160""
                        PlaceholderText=""Type your message…""/>

                    <StackPanel Grid.Row=""1"" Orientation=""Horizontal"" Spacing=""8"" Margin=""0,4,0,0"">
                        <CheckBox Content=""Stream responses"" IsChecked=""True""/>
                        <CheckBox Content=""Enter to send"" IsChecked=""True""/>
                    </StackPanel>
                </Grid>

                <StackPanel Grid.Column=""1"" Orientation=""Horizontal"" Spacing=""8"" VerticalAlignment=""Bottom"">
                    <Button Content=""Templates""/>
                    <Button Content=""Send"" Background=""{StaticResource AccentPurpleBrush}"" Foreground=""White""/>
                </StackPanel>
            </Grid>
        </Border>
    </Grid>
</Page>
"@ | Set-Content -Path (Join-Path $views "ChatPage.xaml") -Encoding UTF8
Write-Ok "Views\ChatPage.xaml written"

Write-Step "Writing Views\ChatPage.xaml.cs"
@"
using Microsoft.UI.Xaml.Controls;

namespace OllamaHub.Views;

public sealed partial class ChatPage : Page
{
    public ChatPage()
    {
        this.InitializeComponent();
    }
}
"@ | Set-Content -Path (Join-Path $views "ChatPage.xaml.cs") -Encoding UTF8
Write-Ok "Views\ChatPage.xaml.cs written"

# COPILOT PAGE (VERY SIMILAR LAYOUT, BUT FRAMED AS INLINE-CODING ASSISTANT)
Write-Step "Writing Views\CopilotPage.xaml"
@"
<Page
    x:Class=""OllamaHub.Views.CopilotPage""
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    mc:Ignorable=""d"">

    <Page.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary Source=""ms-appx:///Styles/AppTheme.xaml""/>
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Page.Resources>

    <Grid Background=""{StaticResource AppBackgroundDarkBrush}"">
        <Grid.RowDefinitions>
            <RowDefinition Height=""Auto""/>
            <RowDefinition Height=""*""/>
            <RowDefinition Height=""Auto""/>
        </Grid.RowDefinitions>

        <!-- HEADER -->
        <Grid Grid.Row=""0"" Background=""{StaticResource SurfaceLevel2Brush}"" Padding=""12,8"">
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width=""*""/>
                <ColumnDefinition Width=""Auto""/>
            </Grid.ColumnDefinitions>

            <StackPanel Orientation=""Horizontal"" Spacing=""8"">
                <TextBlock Text=""Copilot Workspace"" FontSize=""18"" FontWeight=""SemiBold"" Foreground=""White""/>
                <TextBlock Text=""·"" Margin=""4,0"" Foreground=""#9CA3AF""/>
                <TextBlock Text=""Inline code assistant, patch previews, test suggestions"" Foreground=""#9CA3AF""/>
            </StackPanel>

            <StackPanel Grid.Column=""1"" Orientation=""Horizontal"" Spacing=""8"">
                <ComboBox Width=""180"" PlaceholderText=""Copilot model / profile""/>
                <ComboBox Width=""140"" PlaceholderText=""Mode (Fix / Explain / Refactor)""/>
                <Button Content=""History""/>
            </StackPanel>
        </Grid>

        <!-- BODY: LEFT = CODE CONTEXT, RIGHT = SUGGESTIONS / PATCHES -->
        <Grid Grid.Row=""1"">
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width=""*"" MinWidth=""360""/>
                <ColumnDefinition Width=""5""/>
                <ColumnDefinition Width=""*"" MinWidth=""360""/>
            </Grid.ColumnDefinitions>

            <!-- LEFT: CODE CONTEXT -->
            <Border Grid.Column=""0"" Background=""{StaticResource SurfaceLevel1Brush}"" BorderBrush=""{StaticResource BorderBrushSoft}"" BorderThickness=""0,0,1,0"">
                <Grid Padding=""10"">
                    <Grid.RowDefinitions>
                        <RowDefinition Height=""Auto""/>
                        <RowDefinition Height=""*""/>
                        <RowDefinition Height=""Auto""/>
                    </Grid.RowDefinitions>

                    <StackPanel Orientation=""Horizontal"" Spacing=""8"">
                        <TextBlock Text=""Code Context"" FontWeight=""SemiBold"" Foreground=""White""/>
                        <TextBlock Text=""(file, selection, errors)"" Foreground=""#9CA3AF""/>
                    </StackPanel>

                    <ScrollViewer Grid.Row=""1"" Margin=""0,6,0,0"">
                        <StackPanel Spacing=""8"">
                            <Border Background=""{StaticResource SurfaceLevel2Brush}"" Padding=""8"" CornerRadius=""8"">
                                <TextBlock Text=""(Bind: current file path, language, etc.)"" Foreground=""#E5E7EB""/>
                            </Border>

                            <Border Background=""{StaticResource SurfaceLevel2Brush}"" Padding=""8"" CornerRadius=""8"" Margin=""0,4,0,0"">
                                <TextBlock Text=""(Bind: current selection / snippet preview)"" Foreground=""#E5E7EB"" TextWrapping=""Wrap""/>
                            </Border>

                            <Border Background=""{StaticResource SurfaceLevel2Brush}"" Padding=""8"" CornerRadius=""8"" Margin=""0,4,0,0"">
                                <TextBlock Text=""(Bind: compiler / runtime errors, diagnostics)"" Foreground=""#F97373"" TextWrapping=""Wrap""/>
                            </Border>
                        </StackPanel>
                    </ScrollViewer>

                    <StackPanel Grid.Row=""2"" Orientation=""Horizontal"" Spacing=""8"" Margin=""0,8,0,0"">
                        <Button Content=""Refresh context""/>
                        <Button Content=""Attach snippet…""/>
                    </StackPanel>
                </Grid>
            </Border>

            <!-- SPLITTER -->
            <GridSplitter Grid.Column=""1"" HorizontalAlignment=""Stretch"" VerticalAlignment=""Stretch"" Background=""{StaticResource BorderBrushSoft}"" ResizeBehavior=""PreviousAndNext"" ResizeDirection=""Columns""/>

            <!-- RIGHT: SUGGESTIONS / PATCHES -->
            <Grid Grid.Column=""2"">
                <Grid.RowDefinitions>
                    <RowDefinition Height=""*""/>
                </Grid.RowDefinitions>

                <ScrollViewer VerticalScrollBarVisibility=""Auto"">
                    <StackPanel Spacing=""10"" Margin=""0,4,0,0"">

                        <TextBlock Text=""Suggestions & Patches"" FontWeight=""SemiBold"" Foreground=""White""/>

                        <!-- Example suggestion card -->
                        <Border Background=""{StaticResource SurfaceLevel2Brush}"" CornerRadius=""10"" Padding=""10"">
                            <StackPanel Spacing=""6"">
                                <TextBlock Text=""Suggested fix #1"" Foreground=""#E5E7EB"" FontWeight=""SemiBold""/>
                                <TextBlock Text=""(Bind: explanation of what the fix does and why)"" Foreground=""#9CA3AF"" TextWrapping=""Wrap""/>

                                <Border Background=""#111827"" CornerRadius=""6"" Padding=""8"" Margin=""0,4,0,0"">
                                    <TextBlock Text=""(Bind: code patch / diff preview)"" Foreground=""#E5E7EB"" FontFamily=""Consolas"" TextWrapping=""Wrap""/>
                                </Border>

                                <StackPanel Orientation=""Horizontal"" Spacing=""8"" Margin=""0,6,0,0"">
                                    <Button Content=""Apply patch""/>
                                    <Button Content=""Copy patch""/>
                                    <Button Content=""Open in editor""/>
                                </StackPanel>
                            </StackPanel>
                        </Border>

                        <!-- Example tests suggestion -->
                        <Border Background=""{StaticResource SurfaceLevel2Brush}"" CornerRadius=""10"" Padding=""10"">
                            <StackPanel Spacing=""6"">
                                <TextBlock Text=""Suggested tests"" Foreground=""#E5E7EB"" FontWeight=""SemiBold""/>
                                <TextBlock Text=""(Bind: generated unit tests summary)"" Foreground=""#9CA3AF"" TextWrapping=""Wrap""/>
                            </StackPanel>
                        </Border>

                    </StackPanel>
                </ScrollViewer>
            </Grid>
        </Grid>

        <!-- FOOTER: MODE + ACTIONS -->
        <Border Grid.Row=""2"" Background=""{StaticResource SurfaceLevel2Brush}"" Padding=""10,8"">
            <Grid>
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width=""*""/>
                    <ColumnDefinition Width=""Auto""/>
                </Grid.ColumnDefinitions>

                <StackPanel Orientation=""Horizontal"" Spacing=""8"">
                    <ComboBox Width=""180"" PlaceholderText=""Strategy (Conservative / Bold / Experimental)""/>
                    <CheckBox Content=""Auto-generate tests"" IsChecked=""True""/>
                    <CheckBox Content=""Show diffs only""/>
                </StackPanel>

                <StackPanel Grid.Column=""1"" Orientation=""Horizontal"" Spacing=""8"" HorizontalAlignment=""Right"">
                    <Button Content=""Re-run""/>
                    <Button Content=""New suggestion"" Background=""{StaticResource AccentTealBrush}"" Foreground=""Black""/>
                </StackPanel>
            </Grid>
        </Border>
    </Grid>
</Page>
"@ | Set-Content -Path (Join-Path $views "CopilotPage.xaml") -Encoding UTF8
Write-Ok "Views\CopilotPage.xaml written"

Write-Step "Writing Views\CopilotPage.xaml.cs"
@"
using Microsoft.UI.Xaml.Controls;

namespace OllamaHub.Views;

public sealed partial class CopilotPage : Page
{
    public CopilotPage()
    {
        this.InitializeComponent();
    }
}
"@ | Set-Content -Path (Join-Path $views "CopilotPage.xaml.cs") -Encoding UTF8
Write-Ok "Views\CopilotPage.xaml.cs written"

Write-Host "`nAll UI files written. Wire ChatPage and CopilotPage into your main shell/navigation to see them." -ForegroundColor Green
