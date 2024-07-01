# Sideway Speed

Jest to kooperacyjna gra 3D dla dwóch graczy, opracowana jako projekt grupowy na zaliczenie przedmiotu PTG (Podstawy Tworzenia Gier).
Projekt został zrealizowany w zespole trzyosobowym. W tym opisie skupię się na elementach, za które byłem odpowiedzialny.
![cars and colleecti](https://github.com/ErGrejt/Sideway-Speed/assets/127218828/b3c5a689-dff3-4a88-9c6a-db36851f8924)

## Przegląd Projektu

Gra została stworzona przy użyciu Unity i jest zaprojektowana, aby dostarczać angażujące doświadczenie kooperacyjne dla dwóch graczy.
Każdy gracz kontroluje swój samochód i musi współpracować z drugim graczem, aby zebrać wszystkie potrzebne znajdźki. 
Gracze mogą zdobywać punkty za drift, w zależności od długości i prędkości driftu.
Dodatkowo zostały dodane gwiazdki, które są zależne od czasu przejścia poziomu.

## Mój Wkład

### Mechanika Rozgrywki
Byłem odpowiedzialny za projektowanie i implementację głównych mechanik rozgrywki, w tym:

- **Implementacje NetCode**: Implementację oraz konfigurację biblioteki **NetCode** odpowiedzialnej za multiplayer
- **Synchronizację animacji pojazdów graczy**: Stworzenie skryptów dzięki którym gracze widzieli się nawzajem w odpowiednich pozycjach, z odpowiednio ustawionymi kołami, animacją dymu podczas driftowania oraz śladami opon.
- **Synchronizację ruchomych elementów kooperacyjnych**: Stworzenie skryptów i synchronizacja elementów, aby gracze widzieli ruchome elementy w tych samych miejscach i aby przesuwały się one razem z graczem, gdy ten na nich stoi.
- **Swobodny ruch kamery**: Implementacja skryptu odpowiedzialnego za swobodny ruch kamery wokół pojazdu.
- **Skróty klawiszowe**: Implementacja skryptów umożliwiających graczowi, np. przeniesienie pojazdu w bezpieczne miejsce po wciśnięciu **"R"**,
 resetowanie pozycji kamery po wciśnięciu **"V"**, oraz zmienianie odległości rysowania kamery za pomocą **PageDown** i **PageUp**.

### Projektowanie Poziomów
Pracowałem nad tworzeniem i udoskonalaniem kilku elementów gry:

- **Elementy zagadek**: Projektowanie kooperacyjnych zagadek, które wymagają współpracy obu graczy do rozwiązania. Dodatkowo zaprojektowanie portalu który teleportuje gracza w inne miejsce.
- **Implementacja zagadek w scenie**: Usytuowanie elementów kooperacyjnych tak, aby gracz nie miał szans przejść gry w pojedynkę.
- **Prefab znajdziek** - Zaprojektowanie prefaba znajdziek, napisanie skryptu odpowiedzialnego za usuwanie ich ze sceny oraz aktualizacji UI (wspólne dla dwóch graczy) wyświetlającego ile znajdziek zostało zebranych.

### Interfejs Użytkownika
Zajmowałem się rozwojem interfejsu użytkownika, który obejmuje:

- **Elementy HUD**: Stworzenie UI które wyświetla chwilowe informacje na ekranie, np. informacja o ustawieniu nowego checkpointa oraz stworzenie UI wyświetlającego ilość zebranych znajdziek.
- **Główne Menu**: Projektowanie głównego menu z przyciskiem do zakładania gry, przyciskiem do dołączania do gry na wpisany adres IP oraz polem tekstowym wyświetlającym nasze IP.
- **Końcowy ekran**: Zaprojektowanie i usytowanie ekranu końcowego który pojawia się po zebraniu wszystkich znajdziek.

## Technologie Użyte

- **Unity**: Główna platforma deweloperska używana do tworzenia gry.
- **C#**: Język programowania używany do skryptowania mechaniki rozgrywki i elementów interfejsu użytkownika.
- **Unity Version Control**: System kontroli wersji używany do pracy zespołowej nad projektem.

## Autor
Powyższe elementy zostały stworzone przez:
- Witold Woźniczka
## Wykorzystane Assety
- **Samochód** - [Prometeo Car](https://assetstore.unity.com/packages/tools/physics/prometeo-car-controller-209444)
- **Drogi oraz budynki** - [Low Poly Road Pack](https://assetstore.unity.com/packages/3d/environments/roadways/low-poly-road-pack-67288)

## Galeria
<img src="https://github.com/ErGrejt/Sideway-Speed/assets/127218828/b47c94ca-75e7-4043-828f-f4bdf5c70928" width="20%"></img>
<img src="https://github.com/ErGrejt/Sideway-Speed/assets/127218828/f7331228-4461-4b35-a895-9d4492fa9434" width="20%"></img>
<img src="https://github.com/ErGrejt/Sideway-Speed/assets/127218828/8c361984-c951-4843-8faa-78e49afff57d" width="20%"></img>
<img src="https://github.com/ErGrejt/Sideway-Speed/assets/127218828/641f303e-a82a-4295-b309-eb9da0a524f2" width="20%"></img>
<img src="https://github.com/ErGrejt/Sideway-Speed/assets/127218828/e04caef1-4ede-4bef-86f6-990f1b9986c8" width="20%"></img>
<img src="https://github.com/ErGrejt/Sideway-Speed/assets/127218828/15921912-783d-4659-b0ad-8bafed6cfde3" width="20%"></img>
<img src="https://github.com/ErGrejt/Sideway-Speed/assets/127218828/fa05aac0-162c-49ba-80be-aef0bb7d630d" width="20%"></img>
<img src="https://github.com/ErGrejt/Sideway-Speed/assets/127218828/0a6af8b8-0243-46b8-8812-8ec4390ed8d2" width="20%"></img>
<img src="https://github.com/ErGrejt/Sideway-Speed/assets/127218828/6b3ed84c-a0c7-4bb0-9602-f2e3f2b4d909" width="20%"></img>

