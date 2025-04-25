
## BubbleApp UI

Diese Übersicht zeigt den Bestell- und Zahlungsablauf der BubbleApp anhand verschiedener Screenshots und Mockups. Gezeigt wird dabei sowohl die geplante zukünftige Benutzeroberfläche als auch frühere Entwürfe und das aktuelle MVP-Design. Ziel ist es, die Nutzerführung, die Produktauswahl und die Integration von Blockchain-basierten Zahlungen möglichst nachvollziehbar darzustellen – vom ersten Klick bis zur erfolgreichen Transaktion.

### Zukünftiges Home View (Mockup)

Die Startansicht der App im finalen Designentwurf präsentiert das Produktsortiment mit klaren Bildern und Preisen. Nutzer können hier ihre Auswahl treffen, Mengen anpassen und Produkte direkt zum Warenkorb hinzufügen. Das Menü auf der linken Seite ermöglicht schnellen Zugriff auf Warenkorb, Bestellverlauf, Einstellungen etc.

{{~
    futuremockup = temp_file.load_and_link "E1. FutureMockup.png"
~}}
[futuremockup]: {{futuremockup}}  "Future Home View"
![][futuremockup]{style="width:100%;"}

### Create Your Moment – Wireframe (früher Entwurf)

Diese vereinfachte Version des Home View diente als Grundlage für das visuelle Konzept der MVP. Die Benutzeroberfläche ermöglicht die Auswahl von Produkten sowie die Erstellung eigener „Moments“. Die rechte Seite zeigt eine kompakte Zusammenfassung der Bestellung, ergänzt um den Solana-Wert und einen Button zur QR-Code-Erzeugung.

{{~
    earlywireframe = temp_file.load_and_link "E1. HomeViewMockup.png"
~}}
[earlywireframe]: {{earlywireframe}}  "Home View Basis"
![][earlywireframe]{style="width:100%;"}

### Main Page View (aktuelle MVP-Ansicht)

Die Hauptansicht der MVP-Version erlaubt die Auswahl und Anpassung von Produkten über intuitive Plus- und Minus-Buttons. Die aktuelle Bestellung wird in Echtzeit auf der rechten Seite angezeigt, inklusive Summenberechnung in Euro und SOL. Über einen Button kann ein QR-Code zur Zahlung generiert werden. Die Wechselkurse werden dynamisch über die CoinGecko API bezogen.

{{~
    mainpage = temp_file.load_and_link "E1. MainPageView.png"
~}}
[mainpage]: {{mainpage}}  "Main Page View"
![][mainpage]{style="width:100%;"}

### Create Your Own Moment (Live-Funktion im MVP)
Während des Bestellvorgangs können Nutzer individuelle „Moments“ definieren, indem ein Name und ein Preis eingegeben werden. Die so erstellten Produkte lassen sich direkt in die aktuelle Bestellung integrieren und erweitern das Menü flexibel um personalisierte Artikel.

{{~
    cyom = temp_file.load_and_link "E1. CreateYourOwnMoment.png"
~}}
[cyom]: {{cyom}}  "Create Your Own Moment"
![][cyom]{style="width:100%;"}

### Order Confirmation Page (Warten auf Zahlung)
Diese Ansicht zeigt die finale Bestellübersicht sowie einen generierten QR-Code, der alle nötigen Transaktionsdaten enthält. Die BubbleApp wartet in diesem Zustand aktiv auf den Eingang der Zahlung und zeigt visuell an, dass noch keine Bestätigung erfolgt ist.

{{~
    ocp = temp_file.load_and_link "E1. Zahlungwartet.png"
~}}
[ocp]: {{ocp}}  "Order Confirmation Page"
![][ocp]{style="width:100%;"}

### Zahlung mit Bestätigung (Email Prompt)
Bei erfolgreicher Transaktion erscheint eine Bestätigung mit der Option, eine Quittung per E-Mail zu erhalten.

{{~
    emailfrage = temp_file.load_and_link "E1. EmailNotification.png"
~}}
[emailfrage]: {{emailfrage}}  "Bezahlung Bestätigt"
![][emailfrage]{style="width:100%;"}

### Bestellverlauf (Order History)
Diese Ansicht zeigt eine chronologische Übersicht aller bisherigen Bestellungen mit Bestell-ID, Artikelliste, Betrag in Euro und SOL sowie dem Transaktionsstatus. Farblich hervorgehobene Statusanzeigen („Confirmed“ oder „Failed“) und die Anzeige des Transaktions-Hash ermöglichen eine transparente Nachverfolgung der Zahlungen.

{{~
    db = temp_file.load_and_link "E1. Bestellverlauf.png"
~}}
[db]: {{db}}  "Datenbank View"
![][db]{style="width:100%;"}
