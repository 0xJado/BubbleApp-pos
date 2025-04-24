{{~
	include "kiroku/idesis-report.css"
    include "service:kiroku/modules/helper.md"
	include "service:kiroku/modules/comments.md"

	info = MakeReport {
		date: "03. April 2025",
		type: "Dokumentation zur betrieblichen Projektarbeit",
		project: "Erstellung einer Anwendung, die sichere und sofortige Kryptozahlungen vor Ort ermöglicht.",

		author: "Anthony Dreibholz-Ortiz",
		jobtitle: "Fachinformatiker für Anwendungsentwicklung",
		company: "idesis GmbH",
		email: "jado@idesis.de",

		watermark: "Entwurf",
	}
	info.title = "Dokumentation zur betrieblichen Projektarbeit"

	BeginFrontmatter info 
		CommentStyle
	EndFrontmatter

	BeginDocument info
~}}

::::::chapters
{{~
    include "./1. Einleitung.md"
    include "./2. Ausgangssituation.md"
    include "./3. Projektverlauf.md"
	include "./4. Projektdurchführung.md"
~}}
::::::

<style>
  .appendices {
    counter-reset: h1counter h2counter h3counter h4counter h5counter h6counter;
  }

  .my-comment {
	background-color:green;
  }
</style>

::::::appendices

{{PageBreak}}
# Anhang

{{~
    include "./A. Pflichtenheft.md"
	PageBreak
    include "./B. Use-Case-Diagramm.md"
	PageBreak
    include "./C. Aktivitaetsdiagramm.md"
	PageBreak
    include "./D. DV-Konzept.md"
	PageBreak
    include "./F. Tests.md"
	PageBreak
    include "./H. Codeausschnitte.md"
~}}

::::::

{{~
    EndDocument
~}}