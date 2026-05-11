param(
    [Parameter(Mandatory = $true)]
    [string]$InputPath,

    [Parameter(Mandatory = $true)]
    [string]$OutputPath
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Escape-XmlText {
    param([string]$Text)

    if ($null -eq $Text) { return "" }

    $escaped = $Text.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;")
    return $escaped
}

function New-ParagraphXml {
    param(
        [string]$Text,
        [string]$Style = "",
        [switch]$PreserveSpace
    )

    if ([string]::IsNullOrEmpty($Text)) {
        return "<w:p/>"
    }

    $styleXml = ""
    if (-not [string]::IsNullOrWhiteSpace($Style)) {
        $styleXml = "<w:pPr><w:pStyle w:val=`"$Style`"/></w:pPr>"
    }

    $spaceAttr = ""
    if ($PreserveSpace.IsPresent) {
        $spaceAttr = " xml:space=`"preserve`""
    }

    $escaped = Escape-XmlText $Text
    return "<w:p>$styleXml<w:r><w:t$spaceAttr>$escaped</w:t></w:r></w:p>"
}

function Convert-MarkdownLinesToWordXml {
    param([string[]]$Lines)

    $paragraphs = New-Object System.Collections.Generic.List[string]
    $inCodeBlock = $false

    foreach ($line in $Lines) {
        if ($line -match '^```') {
            $inCodeBlock = -not $inCodeBlock
            continue
        }

        if ($inCodeBlock) {
            $paragraphs.Add((New-ParagraphXml -Text $line -Style "Code" -PreserveSpace))
            continue
        }

        if ($line -match '^(#{1,6})\s+(.*)$') {
            $level = $matches[1].Length
            $text = $matches[2]
            $style = switch ($level) {
                1 { "Heading1" }
                2 { "Heading2" }
                3 { "Heading3" }
                4 { "Heading4" }
                default { "Heading5" }
            }
            $paragraphs.Add((New-ParagraphXml -Text $text -Style $style))
            continue
        }

        if ($line -match '^\s*[-*]\s+(.*)$') {
            $paragraphs.Add((New-ParagraphXml -Text ("• " + $matches[1]) -PreserveSpace))
            continue
        }

        if ($line -match '^\s*\d+\.\s+(.*)$') {
            $numberText = $line.Trim()
            $paragraphs.Add((New-ParagraphXml -Text $numberText -PreserveSpace))
            continue
        }

        $normalized = $line -replace '`', ''
        $paragraphs.Add((New-ParagraphXml -Text $normalized -PreserveSpace))
    }

    return ($paragraphs -join "`n")
}

$inputFullPath = [System.IO.Path]::GetFullPath($InputPath)
$outputFullPath = [System.IO.Path]::GetFullPath($OutputPath)
$workingDir = Join-Path ([System.IO.Path]::GetDirectoryName($outputFullPath)) ([System.IO.Path]::GetFileNameWithoutExtension($outputFullPath) + "_docx_tmp")

if (-not (Test-Path $inputFullPath)) {
    throw "Input file not found: $inputFullPath"
}

if (Test-Path $workingDir) {
    Remove-Item -LiteralPath $workingDir -Recurse -Force
}

New-Item -ItemType Directory -Path $workingDir | Out-Null
New-Item -ItemType Directory -Path (Join-Path $workingDir "_rels") | Out-Null
New-Item -ItemType Directory -Path (Join-Path $workingDir "word") | Out-Null

$lines = Get-Content -LiteralPath $inputFullPath -Encoding UTF8
$bodyXml = Convert-MarkdownLinesToWordXml -Lines $lines

$contentTypes = @"
<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<Types xmlns="http://schemas.openxmlformats.org/package/2006/content-types">
  <Default Extension="rels" ContentType="application/vnd.openxmlformats-package.relationships+xml"/>
  <Default Extension="xml" ContentType="application/xml"/>
  <Override PartName="/word/document.xml" ContentType="application/vnd.openxmlformats-officedocument.wordprocessingml.document.main+xml"/>
  <Override PartName="/word/styles.xml" ContentType="application/vnd.openxmlformats-officedocument.wordprocessingml.styles+xml"/>
</Types>
"@

$rels = @"
<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">
  <Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument" Target="word/document.xml"/>
</Relationships>
"@

$styles = @"
<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<w:styles xmlns:w="http://schemas.openxmlformats.org/wordprocessingml/2006/main">
  <w:style w:type="paragraph" w:default="1" w:styleId="Normal">
    <w:name w:val="Normal"/>
    <w:qFormat/>
  </w:style>
  <w:style w:type="paragraph" w:styleId="Heading1">
    <w:name w:val="heading 1"/>
    <w:basedOn w:val="Normal"/>
    <w:next w:val="Normal"/>
    <w:qFormat/>
    <w:rPr><w:b/><w:sz w:val="32"/></w:rPr>
  </w:style>
  <w:style w:type="paragraph" w:styleId="Heading2">
    <w:name w:val="heading 2"/>
    <w:basedOn w:val="Normal"/>
    <w:next w:val="Normal"/>
    <w:qFormat/>
    <w:rPr><w:b/><w:sz w:val="28"/></w:rPr>
  </w:style>
  <w:style w:type="paragraph" w:styleId="Heading3">
    <w:name w:val="heading 3"/>
    <w:basedOn w:val="Normal"/>
    <w:next w:val="Normal"/>
    <w:qFormat/>
    <w:rPr><w:b/><w:sz w:val="24"/></w:rPr>
  </w:style>
  <w:style w:type="paragraph" w:styleId="Heading4">
    <w:name w:val="heading 4"/>
    <w:basedOn w:val="Normal"/>
    <w:next w:val="Normal"/>
    <w:qFormat/>
    <w:rPr><w:b/><w:sz w:val="22"/></w:rPr>
  </w:style>
  <w:style w:type="paragraph" w:styleId="Heading5">
    <w:name w:val="heading 5"/>
    <w:basedOn w:val="Normal"/>
    <w:next w:val="Normal"/>
    <w:qFormat/>
    <w:rPr><w:b/><w:sz w:val="20"/></w:rPr>
  </w:style>
  <w:style w:type="paragraph" w:styleId="Code">
    <w:name w:val="Code"/>
    <w:basedOn w:val="Normal"/>
    <w:pPr><w:spacing w:before="0" w:after="0"/></w:pPr>
    <w:rPr>
      <w:rFonts w:ascii="Consolas" w:hAnsi="Consolas"/>
      <w:sz w:val="20"/>
    </w:rPr>
  </w:style>
</w:styles>
"@

$document = @"
<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<w:document xmlns:wpc="http://schemas.microsoft.com/office/word/2010/wordprocessingCanvas"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    xmlns:o="urn:schemas-microsoft-com:office:office"
    xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships"
    xmlns:m="http://schemas.openxmlformats.org/officeDocument/2006/math"
    xmlns:v="urn:schemas-microsoft-com:vml"
    xmlns:wp14="http://schemas.microsoft.com/office/word/2010/wordprocessingDrawing"
    xmlns:wp="http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing"
    xmlns:w10="urn:schemas-microsoft-com:office:word"
    xmlns:w="http://schemas.openxmlformats.org/wordprocessingml/2006/main"
    xmlns:w14="http://schemas.microsoft.com/office/word/2010/wordml"
    xmlns:wpg="http://schemas.microsoft.com/office/word/2010/wordprocessingGroup"
    xmlns:wpi="http://schemas.microsoft.com/office/word/2010/wordprocessingInk"
    xmlns:wne="http://schemas.microsoft.com/office/word/2006/wordml"
    xmlns:wps="http://schemas.microsoft.com/office/word/2010/wordprocessingShape"
    mc:Ignorable="w14 wp14">
  <w:body>
$bodyXml
    <w:sectPr>
      <w:pgSz w:w="11906" w:h="16838"/>
      <w:pgMar w:top="1440" w:right="1440" w:bottom="1440" w:left="1440" w:header="708" w:footer="708" w:gutter="0"/>
    </w:sectPr>
  </w:body>
</w:document>
"@

Set-Content -LiteralPath (Join-Path $workingDir "[Content_Types].xml") -Value $contentTypes -Encoding UTF8
Set-Content -LiteralPath (Join-Path $workingDir "_rels\\.rels") -Value $rels -Encoding UTF8
Set-Content -LiteralPath (Join-Path $workingDir "word\\styles.xml") -Value $styles -Encoding UTF8
Set-Content -LiteralPath (Join-Path $workingDir "word\\document.xml") -Value $document -Encoding UTF8

if (Test-Path $outputFullPath) {
    Remove-Item -LiteralPath $outputFullPath -Force
}

$zipPath = [System.IO.Path]::ChangeExtension($outputFullPath, ".zip")
if (Test-Path $zipPath) {
    Remove-Item -LiteralPath $zipPath -Force
}

Compress-Archive -Path (Join-Path $workingDir "*") -DestinationPath $zipPath -Force
Move-Item -LiteralPath $zipPath -Destination $outputFullPath -Force
Remove-Item -LiteralPath $workingDir -Recurse -Force

Write-Host "Created: $outputFullPath"
