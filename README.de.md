# OkTyles

[![Code Validation](https://github.com/SameplayerDE/OkTyles/actions/workflows/compile.yml/badge.svg)](https://github.com/SameplayerDE/OkTyles/actions/workflows/compile.yml)

Deutsch | [English](README.md)

OkTyles ist ein Karten-Editor, bei dem jede Zelle innerhalb der Karte ihre eigene Rotation, Kollision und Bildinformationen hat. Dies ermöglicht eine präzise Kontrolle über jede Zelle und reduziert den Speicherbedarf auf 32 Bits pro Zelle für eine kompakte Darstellung.

## Features

- Jede Zelle enthält Rotation, Kollision und Anzeigeinformationen.
- Kompakte Speicherung mit nur 32 Bits pro Zelle.
- Bewegen Sie sich frei zwischen den Kacheln für einfache Navigation.
- Individuelle Rotation pro Zelle reduziert die Anzahl der benötigten Kacheln.
- Layer-Verwaltung zur Organisation und Steuerung der Tiefe von Elementen in der Karte.

## Todo

- Implementierung von Lua-Skriptunterstützung.
- Unterstützung für benutzerdefinierte Speicherstruktur der Karte hinzufügen.
- Einführung der Funktionalität für animierte Kacheln.
- Implementierung von Kachelregeln für dynamischere Karteninteraktionen.

## Screenshots

![Karte bearbeiten](Assets/image0.PNG)
*Bearbeiten Sie die Karte direkt im Editor.*

![Freie Bewegung](Assets/image1.PNG)
*Bewegen Sie sich frei zwischen den Zellen.*

## Installation

1. Klone das Repository: `git clone https://github.com/SameplayerDE/OkTyles.git`
2. Öffne das Projekt in deiner bevorzugten Entwicklungsumgebung.
3. Führe das Projekt aus und beginne den Karten-Editor zu nutzen.

## Tastenkürzel

- **Zoomen**: `Maus Scrollen`
- **Verschieben**: `Maus-Taste Mitte`
- **Rückgängig machen**: `Strg + Z`
- **Spiegelzustand umschalten**: `Strg + R` (Halte `Shift` gedrückt, um den Spiegelzustand anzuzeigen)
- **Alle Layer anzeigen**: `Strg + Rechte Umschalttaste`
- **Aktiven Layer nach oben wechseln**: `Strg + Hoch-Pfeil`
- **Aktiven Layer nach unten wechseln**: `Strg + Runter-Pfeil`

## Beitrag

Fühlst du dich inspiriert? Möchtest du den Karten-Editor verbessern oder erweitern? Beiträge sind willkommen! Öffne einfach ein Pull-Request mit deinen vorgeschlagenen Änderungen.

## Lizenz

Dieses Projekt ist unter der MIT-Lizenz lizenziert. Weitere Informationen findest du in der [LICENSE](https://github.com/SameplayerDE/OkTyles/blob/master/LICENSE)-Datei.
