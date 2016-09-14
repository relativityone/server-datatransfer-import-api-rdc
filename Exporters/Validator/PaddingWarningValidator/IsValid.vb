Imports NUnit.Framework
Namespace kCura.WinEDDS.NUnit.Exporters.Validator.PaddingWarningValidator

	<TestFixture()> Public Class IsValid
		Private _settings As ExportFile
		Private _validator As WinEDDS.Exporters.Validator.PaddingWarningValidator
		Private _recommendedVolumePadding As Int32
		Private _recommendedSubDirPadding As Int32
		Private _actual As Boolean

		<Test()>
		<TestCase(True, False, False, False, False, False, False, False, False, Description:="1")>
		<TestCase(True, False, False, False, False, False, False, False, True, Description:="2")>
		<TestCase(True, False, False, False, False, False, False, True, False, Description:="3")>
		<TestCase(True, False, False, False, False, False, False, True, True, Description:="4")>
		<TestCase(True, False, False, False, False, False, True, False, False, Description:="5")>
		<TestCase(True, False, False, False, False, False, True, False, True, Description:="6")>
		<TestCase(True, False, False, False, False, False, True, True, False, Description:="7")>
		<TestCase(True, False, False, False, False, False, True, True, True, Description:="8")>
		<TestCase(True, False, False, False, False, True, False, False, False, Description:="9")>
		<TestCase(True, False, False, False, False, True, False, False, True, Description:="10")>
		<TestCase(True, False, False, False, False, True, False, True, False, Description:="11")>
		<TestCase(True, False, False, False, False, True, False, True, True, Description:="12")>
		<TestCase(True, False, False, False, False, True, True, False, False, Description:="13")>
		<TestCase(True, False, False, False, False, True, True, False, True, Description:="14")>
		<TestCase(True, False, False, False, False, True, True, True, False, Description:="15")>
		<TestCase(True, False, False, False, False, True, True, True, True, Description:="16")>
		<TestCase(True, False, False, False, True, False, False, False, False, Description:="17")>
		<TestCase(True, False, False, False, True, False, False, False, True, Description:="18")>
		<TestCase(True, False, False, False, True, False, False, True, False, Description:="19")>
		<TestCase(True, False, False, False, True, False, False, True, True, Description:="20")>
		<TestCase(True, False, False, False, True, False, True, False, False, Description:="21")>
		<TestCase(True, False, False, False, True, False, True, False, True, Description:="22")>
		<TestCase(True, False, False, False, True, False, True, True, False, Description:="23")>
		<TestCase(True, False, False, False, True, False, True, True, True, Description:="24")>
		<TestCase(True, False, False, False, True, True, False, False, False, Description:="25")>
		<TestCase(True, False, False, False, True, True, False, False, True, Description:="26")>
		<TestCase(True, False, False, False, True, True, False, True, False, Description:="27")>
		<TestCase(True, False, False, False, True, True, False, True, True, Description:="28")>
		<TestCase(True, False, False, False, True, True, True, False, False, Description:="29")>
		<TestCase(True, False, False, False, True, True, True, False, True, Description:="30")>
		<TestCase(True, False, False, False, True, True, True, True, False, Description:="31")>
		<TestCase(True, False, False, False, True, True, True, True, True, Description:="32")>
		<TestCase(True, False, False, True, False, False, False, False, False, Description:="33")>
		<TestCase(True, False, False, True, False, False, False, False, True, Description:="34")>
		<TestCase(True, False, False, True, False, False, False, True, False, Description:="35")>
		<TestCase(True, False, False, True, False, False, False, True, True, Description:="36")>
		<TestCase(True, False, False, True, False, False, True, False, False, Description:="37")>
		<TestCase(True, False, False, True, False, False, True, False, True, Description:="38")>
		<TestCase(True, False, False, True, False, False, True, True, False, Description:="39")>
		<TestCase(True, False, False, True, False, False, True, True, True, Description:="40")>
		<TestCase(True, False, False, True, False, True, False, False, False, Description:="41")>
		<TestCase(True, False, False, True, False, True, False, False, True, Description:="42")>
		<TestCase(True, False, False, True, False, True, False, True, False, Description:="43")>
		<TestCase(True, False, False, True, False, True, False, True, True, Description:="44")>
		<TestCase(True, False, False, True, False, True, True, False, False, Description:="45")>
		<TestCase(True, False, False, True, False, True, True, False, True, Description:="46")>
		<TestCase(True, False, False, True, False, True, True, True, False, Description:="47")>
		<TestCase(True, False, False, True, False, True, True, True, True, Description:="48")>
		<TestCase(True, False, False, True, True, False, False, False, False, Description:="49")>
		<TestCase(True, False, False, True, True, False, False, False, True, Description:="50")>
		<TestCase(True, False, False, True, True, False, False, True, False, Description:="51")>
		<TestCase(True, False, False, True, True, False, False, True, True, Description:="52")>
		<TestCase(True, False, False, True, True, False, True, False, False, Description:="53")>
		<TestCase(True, False, False, True, True, False, True, False, True, Description:="54")>
		<TestCase(True, False, False, True, True, False, True, True, False, Description:="55")>
		<TestCase(True, False, False, True, True, False, True, True, True, Description:="56")>
		<TestCase(True, False, False, True, True, True, False, False, False, Description:="57")>
		<TestCase(True, False, False, True, True, True, False, False, True, Description:="58")>
		<TestCase(True, False, False, True, True, True, False, True, False, Description:="59")>
		<TestCase(True, False, False, True, True, True, False, True, True, Description:="60")>
		<TestCase(True, False, False, True, True, True, True, False, False, Description:="61")>
		<TestCase(True, False, False, True, True, True, True, False, True, Description:="62")>
		<TestCase(True, False, False, True, True, True, True, True, False, Description:="63")>
		<TestCase(True, False, False, True, True, True, True, True, True, Description:="64")>
		<TestCase(True, False, True, False, False, False, False, False, False, Description:="65")>
		<TestCase(True, False, True, False, False, False, False, False, True, Description:="66")>
		<TestCase(True, False, True, False, False, False, False, True, False, Description:="67")>
		<TestCase(False, False, True, False, False, False, False, True, True, Description:="68")>
		<TestCase(True, False, True, False, False, False, True, False, False, Description:="69")>
		<TestCase(True, False, True, False, False, False, True, False, True, Description:="70")>
		<TestCase(True, False, True, False, False, False, True, True, False, Description:="71")>
		<TestCase(False, False, True, False, False, False, True, True, True, Description:="72")>
		<TestCase(True, False, True, False, False, True, False, False, False, Description:="73")>
		<TestCase(True, False, True, False, False, True, False, False, True, Description:="74")>
		<TestCase(True, False, True, False, False, True, False, True, False, Description:="75")>
		<TestCase(False, False, True, False, False, True, False, True, True, Description:="76")>
		<TestCase(False, False, True, False, False, True, True, False, False, Description:="77")>
		<TestCase(False, False, True, False, False, True, True, False, True, Description:="78")>
		<TestCase(False, False, True, False, False, True, True, True, False, Description:="79")>
		<TestCase(False, False, True, False, False, True, True, True, True, Description:="80")>
		<TestCase(True, False, True, False, True, False, False, False, False, Description:="81")>
		<TestCase(True, False, True, False, True, False, False, False, True, Description:="82")>
		<TestCase(True, False, True, False, True, False, False, True, False, Description:="83")>
		<TestCase(False, False, True, False, True, False, False, True, True, Description:="84")>
		<TestCase(True, False, True, False, True, False, True, False, False, Description:="85")>
		<TestCase(True, False, True, False, True, False, True, False, True, Description:="86")>
		<TestCase(True, False, True, False, True, False, True, True, False, Description:="87")>
		<TestCase(False, False, True, False, True, False, True, True, True, Description:="88")>
		<TestCase(True, False, True, False, True, True, False, False, False, Description:="89")>
		<TestCase(True, False, True, False, True, True, False, False, True, Description:="90")>
		<TestCase(True, False, True, False, True, True, False, True, False, Description:="91")>
		<TestCase(False, False, True, False, True, True, False, True, True, Description:="92")>
		<TestCase(False, False, True, False, True, True, True, False, False, Description:="93")>
		<TestCase(False, False, True, False, True, True, True, False, True, Description:="94")>
		<TestCase(False, False, True, False, True, True, True, True, False, Description:="95")>
		<TestCase(False, False, True, False, True, True, True, True, True, Description:="96")>
		<TestCase(True, False, True, True, False, False, False, False, False, Description:="97")>
		<TestCase(True, False, True, True, False, False, False, False, True, Description:="98")>
		<TestCase(True, False, True, True, False, False, False, True, False, Description:="99")>
		<TestCase(False, False, True, True, False, False, False, True, True, Description:="100")>
		<TestCase(True, False, True, True, False, False, True, False, False, Description:="101")>
		<TestCase(True, False, True, True, False, False, True, False, True, Description:="102")>
		<TestCase(True, False, True, True, False, False, True, True, False, Description:="103")>
		<TestCase(False, False, True, True, False, False, True, True, True, Description:="104")>
		<TestCase(True, False, True, True, False, True, False, False, False, Description:="105")>
		<TestCase(True, False, True, True, False, True, False, False, True, Description:="106")>
		<TestCase(True, False, True, True, False, True, False, True, False, Description:="107")>
		<TestCase(False, False, True, True, False, True, False, True, True, Description:="108")>
		<TestCase(False, False, True, True, False, True, True, False, False, Description:="109")>
		<TestCase(False, False, True, True, False, True, True, False, True, Description:="110")>
		<TestCase(False, False, True, True, False, True, True, True, False, Description:="111")>
		<TestCase(False, False, True, True, False, True, True, True, True, Description:="112")>
		<TestCase(False, False, True, True, True, False, False, False, False, Description:="113")>
		<TestCase(False, False, True, True, True, False, False, False, True, Description:="114")>
		<TestCase(False, False, True, True, True, False, False, True, False, Description:="115")>
		<TestCase(False, False, True, True, True, False, False, True, True, Description:="116")>
		<TestCase(False, False, True, True, True, False, True, False, False, Description:="117")>
		<TestCase(False, False, True, True, True, False, True, False, True, Description:="118")>
		<TestCase(False, False, True, True, True, False, True, True, False, Description:="119")>
		<TestCase(False, False, True, True, True, False, True, True, True, Description:="120")>
		<TestCase(False, False, True, True, True, True, False, False, False, Description:="121")>
		<TestCase(False, False, True, True, True, True, False, False, True, Description:="122")>
		<TestCase(False, False, True, True, True, True, False, True, False, Description:="123")>
		<TestCase(False, False, True, True, True, True, False, True, True, Description:="124")>
		<TestCase(False, False, True, True, True, True, True, False, False, Description:="125")>
		<TestCase(False, False, True, True, True, True, True, False, True, Description:="126")>
		<TestCase(False, False, True, True, True, True, True, True, False, Description:="127")>
		<TestCase(False, False, True, True, True, True, True, True, True, Description:="128")>
		<TestCase(True, True, False, False, False, False, False, False, False, Description:="129")>
		<TestCase(True, True, False, False, False, False, False, False, True, Description:="130")>
		<TestCase(True, True, False, False, False, False, False, True, False, Description:="131")>
		<TestCase(False, True, False, False, False, False, False, True, True, Description:="132")>
		<TestCase(True, True, False, False, False, False, True, False, False, Description:="133")>
		<TestCase(True, True, False, False, False, False, True, False, True, Description:="134")>
		<TestCase(True, True, False, False, False, False, True, True, False, Description:="135")>
		<TestCase(False, True, False, False, False, False, True, True, True, Description:="136")>
		<TestCase(True, True, False, False, False, True, False, False, False, Description:="137")>
		<TestCase(True, True, False, False, False, True, False, False, True, Description:="138")>
		<TestCase(True, True, False, False, False, True, False, True, False, Description:="139")>
		<TestCase(False, True, False, False, False, True, False, True, True, Description:="140")>
		<TestCase(False, True, False, False, False, True, True, False, False, Description:="141")>
		<TestCase(False, True, False, False, False, True, True, False, True, Description:="142")>
		<TestCase(False, True, False, False, False, True, True, True, False, Description:="143")>
		<TestCase(False, True, False, False, False, True, True, True, True, Description:="144")>
		<TestCase(True, True, False, False, True, False, False, False, False, Description:="145")>
		<TestCase(True, True, False, False, True, False, False, False, True, Description:="146")>
		<TestCase(True, True, False, False, True, False, False, True, False, Description:="147")>
		<TestCase(False, True, False, False, True, False, False, True, True, Description:="148")>
		<TestCase(True, True, False, False, True, False, True, False, False, Description:="149")>
		<TestCase(True, True, False, False, True, False, True, False, True, Description:="150")>
		<TestCase(True, True, False, False, True, False, True, True, False, Description:="151")>
		<TestCase(False, True, False, False, True, False, True, True, True, Description:="152")>
		<TestCase(True, True, False, False, True, True, False, False, False, Description:="153")>
		<TestCase(True, True, False, False, True, True, False, False, True, Description:="154")>
		<TestCase(True, True, False, False, True, True, False, True, False, Description:="155")>
		<TestCase(False, True, False, False, True, True, False, True, True, Description:="156")>
		<TestCase(False, True, False, False, True, True, True, False, False, Description:="157")>
		<TestCase(False, True, False, False, True, True, True, False, True, Description:="158")>
		<TestCase(False, True, False, False, True, True, True, True, False, Description:="159")>
		<TestCase(False, True, False, False, True, True, True, True, True, Description:="160")>
		<TestCase(True, True, False, True, False, False, False, False, False, Description:="161")>
		<TestCase(True, True, False, True, False, False, False, False, True, Description:="162")>
		<TestCase(True, True, False, True, False, False, False, True, False, Description:="163")>
		<TestCase(False, True, False, True, False, False, False, True, True, Description:="164")>
		<TestCase(True, True, False, True, False, False, True, False, False, Description:="165")>
		<TestCase(True, True, False, True, False, False, True, False, True, Description:="166")>
		<TestCase(True, True, False, True, False, False, True, True, False, Description:="167")>
		<TestCase(False, True, False, True, False, False, True, True, True, Description:="168")>
		<TestCase(True, True, False, True, False, True, False, False, False, Description:="169")>
		<TestCase(True, True, False, True, False, True, False, False, True, Description:="170")>
		<TestCase(True, True, False, True, False, True, False, True, False, Description:="171")>
		<TestCase(False, True, False, True, False, True, False, True, True, Description:="172")>
		<TestCase(False, True, False, True, False, True, True, False, False, Description:="173")>
		<TestCase(False, True, False, True, False, True, True, False, True, Description:="174")>
		<TestCase(False, True, False, True, False, True, True, True, False, Description:="175")>
		<TestCase(False, True, False, True, False, True, True, True, True, Description:="176")>
		<TestCase(False, True, False, True, True, False, False, False, False, Description:="177")>
		<TestCase(False, True, False, True, True, False, False, False, True, Description:="178")>
		<TestCase(False, True, False, True, True, False, False, True, False, Description:="179")>
		<TestCase(False, True, False, True, True, False, False, True, True, Description:="180")>
		<TestCase(False, True, False, True, True, False, True, False, False, Description:="181")>
		<TestCase(False, True, False, True, True, False, True, False, True, Description:="182")>
		<TestCase(False, True, False, True, True, False, True, True, False, Description:="183")>
		<TestCase(False, True, False, True, True, False, True, True, True, Description:="184")>
		<TestCase(False, True, False, True, True, True, False, False, False, Description:="185")>
		<TestCase(False, True, False, True, True, True, False, False, True, Description:="186")>
		<TestCase(False, True, False, True, True, True, False, True, False, Description:="187")>
		<TestCase(False, True, False, True, True, True, False, True, True, Description:="188")>
		<TestCase(False, True, False, True, True, True, True, False, False, Description:="189")>
		<TestCase(False, True, False, True, True, True, True, False, True, Description:="190")>
		<TestCase(False, True, False, True, True, True, True, True, False, Description:="191")>
		<TestCase(False, True, False, True, True, True, True, True, True, Description:="192")>
		<TestCase(True, True, True, False, False, False, False, False, False, Description:="193")>
		<TestCase(True, True, True, False, False, False, False, False, True, Description:="194")>
		<TestCase(True, True, True, False, False, False, False, True, False, Description:="195")>
		<TestCase(False, True, True, False, False, False, False, True, True, Description:="196")>
		<TestCase(True, True, True, False, False, False, True, False, False, Description:="197")>
		<TestCase(True, True, True, False, False, False, True, False, True, Description:="198")>
		<TestCase(True, True, True, False, False, False, True, True, False, Description:="199")>
		<TestCase(False, True, True, False, False, False, True, True, True, Description:="200")>
		<TestCase(True, True, True, False, False, True, False, False, False, Description:="201")>
		<TestCase(True, True, True, False, False, True, False, False, True, Description:="202")>
		<TestCase(True, True, True, False, False, True, False, True, False, Description:="203")>
		<TestCase(False, True, True, False, False, True, False, True, True, Description:="204")>
		<TestCase(False, True, True, False, False, True, True, False, False, Description:="205")>
		<TestCase(False, True, True, False, False, True, True, False, True, Description:="206")>
		<TestCase(False, True, True, False, False, True, True, True, False, Description:="207")>
		<TestCase(False, True, True, False, False, True, True, True, True, Description:="208")>
		<TestCase(True, True, True, False, True, False, False, False, False, Description:="209")>
		<TestCase(True, True, True, False, True, False, False, False, True, Description:="210")>
		<TestCase(True, True, True, False, True, False, False, True, False, Description:="211")>
		<TestCase(False, True, True, False, True, False, False, True, True, Description:="212")>
		<TestCase(True, True, True, False, True, False, True, False, False, Description:="213")>
		<TestCase(True, True, True, False, True, False, True, False, True, Description:="214")>
		<TestCase(True, True, True, False, True, False, True, True, False, Description:="215")>
		<TestCase(False, True, True, False, True, False, True, True, True, Description:="216")>
		<TestCase(True, True, True, False, True, True, False, False, False, Description:="217")>
		<TestCase(True, True, True, False, True, True, False, False, True, Description:="218")>
		<TestCase(True, True, True, False, True, True, False, True, False, Description:="219")>
		<TestCase(False, True, True, False, True, True, False, True, True, Description:="220")>
		<TestCase(False, True, True, False, True, True, True, False, False, Description:="221")>
		<TestCase(False, True, True, False, True, True, True, False, True, Description:="222")>
		<TestCase(False, True, True, False, True, True, True, True, False, Description:="223")>
		<TestCase(False, True, True, False, True, True, True, True, True, Description:="224")>
		<TestCase(True, True, True, True, False, False, False, False, False, Description:="225")>
		<TestCase(True, True, True, True, False, False, False, False, True, Description:="226")>
		<TestCase(True, True, True, True, False, False, False, True, False, Description:="227")>
		<TestCase(False, True, True, True, False, False, False, True, True, Description:="228")>
		<TestCase(True, True, True, True, False, False, True, False, False, Description:="229")>
		<TestCase(True, True, True, True, False, False, True, False, True, Description:="230")>
		<TestCase(True, True, True, True, False, False, True, True, False, Description:="231")>
		<TestCase(False, True, True, True, False, False, True, True, True, Description:="232")>
		<TestCase(True, True, True, True, False, True, False, False, False, Description:="233")>
		<TestCase(True, True, True, True, False, True, False, False, True, Description:="234")>
		<TestCase(True, True, True, True, False, True, False, True, False, Description:="235")>
		<TestCase(False, True, True, True, False, True, False, True, True, Description:="236")>
		<TestCase(False, True, True, True, False, True, True, False, False, Description:="237")>
		<TestCase(False, True, True, True, False, True, True, False, True, Description:="238")>
		<TestCase(False, True, True, True, False, True, True, True, False, Description:="239")>
		<TestCase(False, True, True, True, False, True, True, True, True, Description:="240")>
		<TestCase(False, True, True, True, True, False, False, False, False, Description:="241")>
		<TestCase(False, True, True, True, True, False, False, False, True, Description:="242")>
		<TestCase(False, True, True, True, True, False, False, True, False, Description:="243")>
		<TestCase(False, True, True, True, True, False, False, True, True, Description:="244")>
		<TestCase(False, True, True, True, True, False, True, False, False, Description:="245")>
		<TestCase(False, True, True, True, True, False, True, False, True, Description:="246")>
		<TestCase(False, True, True, True, True, False, True, True, False, Description:="247")>
		<TestCase(False, True, True, True, True, False, True, True, True, Description:="248")>
		<TestCase(False, True, True, True, True, True, False, False, False, Description:="249")>
		<TestCase(False, True, True, True, True, True, False, False, True, Description:="250")>
		<TestCase(False, True, True, True, True, True, False, True, False, Description:="251")>
		<TestCase(False, True, True, True, True, True, False, True, True, Description:="252")>
		<TestCase(False, True, True, True, True, True, True, False, False, Description:="253")>
		<TestCase(False, True, True, True, True, True, True, False, True, Description:="254")>
		<TestCase(False, True, True, True, True, True, True, True, False, Description:="255")>
		<TestCase(False, True, True, True, True, True, True, True, True, Description:="256")>
		Public Sub GoldenFlow(
								expected As Boolean,
								recommendedVolumeGreater As Boolean,
								recommendedSubDirectoryGreater As Boolean,
								exportFullText As Boolean,
								exportFullTextAsFile As Boolean,
								copyImagesFromRepository As Boolean,
								exportImages As Boolean,
								copyNativesFromRepository As Boolean,
								exportNatives As Boolean
								)
			Arrange(exportFullText, exportFullTextAsFile, recommendedVolumeGreater, recommendedSubDirectoryGreater, copyImagesFromRepository, copyNativesFromRepository, exportImages, exportNatives)
			Act()
			AssertValidator(expected)
		End Sub


		Private Sub Arrange(
   								exportFullText As Boolean,
								exportFullTextAsFile As Boolean,
								recommendedVolumeGreater As Boolean,
								recommendedSubDirectoryGreater As Boolean,
								copyImagesFromRepository As Boolean,
								copyNativesFromRepository As Boolean,
								exportImages As Boolean,
								exportNatives As Boolean
						   )
			_settings = New ExportFile(10)
			_settings.VolumeInfo = New WinEDDS.Exporters.VolumeInfo()
			_settings.ExportFullText = exportFullText
			_settings.ExportFullTextAsFile = exportFullTextAsFile
			_settings.ExportImages = exportImages
			_settings.ExportNative = exportNatives
			_settings.VolumeInfo.CopyImageFilesFromRepository = copyImagesFromRepository
			_settings.VolumeInfo.CopyNativeFilesFromRepository = copyNativesFromRepository
			If recommendedSubDirectoryGreater Then
				_recommendedSubDirPadding = 10
				_settings.SubdirectoryDigitPadding = 9
			Else
				_recommendedSubDirPadding = 9
				_settings.SubdirectoryDigitPadding = 10
			End If
			If recommendedVolumeGreater Then
				_recommendedVolumePadding = 10
				_settings.VolumeDigitPadding = 9
			Else
				_recommendedVolumePadding = 9
				_settings.VolumeDigitPadding = 10
			End If
		End Sub

		Private Sub Act()
			_validator = New WinEDDS.Exporters.Validator.PaddingWarningValidator
			_actual = _validator.IsValid(_settings, _recommendedVolumePadding, _recommendedSubDirPadding)
		End Sub

		Private Sub AssertValidator(expected As Boolean)
			Assert.AreEqual(expected, _actual)
			If Not expected Then
				Assert.IsNotEmpty(_validator.ErrorMessages)
			End If
		End Sub
	End Class
End Namespace
