<?xml version="1.0" ?>
<xsl:stylesheet version="1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:wix="http://schemas.microsoft.com/wix/2006/wi">

  <!-- Copy all attributes and elements to the output. -->
  <xsl:template match="@*|*">
	<xsl:copy>
	  <xsl:apply-templates select="@*" />
	  <xsl:apply-templates select="*" />
	</xsl:copy>
  </xsl:template>

  <xsl:output method="xml" indent="yes" />

  <xsl:key name="winedds-winform-search" match="wix:Component[contains(wix:File/@Source, 'kCura.EDDS.WinForm.exe')]" use="@Id" />
  <xsl:template match="wix:Component[key('winedds-winform-search', @Id)]" />
  <xsl:template match="wix:ComponentRef[key('winedds-winform-search', @Id)]" />
  
  <xsl:key name="freeimage-dll-search" match="wix:Component[contains(wix:File/@Source, 'FreeImage.dll')]" use="@Id" />
  <xsl:template match="wix:Component[key('freeimage-dll-search', @Id)]" />
  <xsl:template match="wix:ComponentRef[key('freeimage-dll-search', @Id)]" />

  <xsl:key name="freeimagenet-dll-search" match="wix:Component[contains(wix:File/@Source, 'FreeImageNET.dll')]" use="@Id" />
  <xsl:template match="wix:Component[key('freeimagenet-dll-search', @Id)]" />
  <xsl:template match="wix:ComponentRef[key('freeimagenet-dll-search', @Id)]" />
  
  <xsl:key name="itextsharp-dll-search" match="wix:Component[contains(wix:File/@Source, 'itextsharp.dll')]" use="@Id" />
  <xsl:template match="wix:Component[key('itextsharp-dll-search', @Id)]" />
  <xsl:template match="wix:ComponentRef[key('itextsharp-dll-search', @Id)]" />
</xsl:stylesheet>