
{#apdx-use-case-diagramm}
## Use Case Diagramm

Das folgende Use Case Diagramm bildet die zentralen Interaktionen zwischen dem BubbleApp System und den beteiligten Akteuren ab. Es dient der Visualisierung funktionaler Anforderungen aus Benutzersicht und macht dabei sowohl optionale als auch externe Prozesse deutlich. Als primäre Akteure agieren der Foodtruck Betreiber sowie der Kunde. Während der Foodtruck Betreiber direkten Zugriff auf alle systeminternen Funktionen hat – etwa das Hinzufügen von Artikeln, das Erstellen benutzerdefinierter Positionen, das Prüfen der Bestellung, das Generieren eines QR Codes sowie die Überwachung des Zahlungseingangs – interagiert der Kunde ausschließlich über externe Schnittstellen. Diese bestehen aus dem Scannen des generierten QR Codes sowie der anschließenden Bestätigung der Transaktion innerhalb seiner eigenen Wallet. Diese externen Aktionen wurden im Diagramm visuell hervorgehoben, um den Systemkontext klar abzugrenzen. Zur Darstellung abhängiger Abläufe kommen „include“ Beziehungen zum Einsatz, etwa zwischen dem Generieren des QR Codes und dem Prüfen der Bestellung oder zwischen der Zahlungsüberwachung und der Speicherung in SQLite. Optional ausführbare Abläufe, wie das Senden einer Email Quittung, sind hingegen durch „extend“ Beziehungen modelliert. Gleiches gilt für die Kundenaktionen, welche für den internen Zahlungsprozess benötigt, jedoch nicht innerhalb der App selbst ausgeführt werden. Durch diese Struktur wird das Zusammenspiel aus Benutzerinteraktion, Systemverantwortung und externen Prozessen klar nachvollziehbar. Das Diagramm bietet somit einen präzisen Überblick über den funktionalen Umfang des MVPs und legt die Grundlage für eine transparente Kommunikation systemrelevanter Abläufe.

{{~
    usecasediagramm = temp_file.load_and_link "B. UseCaseDiagramm.png"
~}}
[usecasediagramm]: {{usecasediagramm}}  "Use Case Diagramm"
![][usecasediagramm]{style="width:100%;"}
