# APBD – Ćwiczenie 7

## Uruchomienie aplikacji

1. Sklonuj repozytorium:

   ```bash
   git clone https://github.com/s29640/apbd-cw7
   cd apbd-cw7
   ```

2. Uruchom skrypt `./database/init.sql` na lokalnej instancji Microsoft SQL Server.

   Skrypt automatycznie:
   - tworzy bazę danych `APBD-CW7`,
   - tworzy tabele `Tickets` i `TicketComments`,
   - tworzy relacje oraz przykładowe dane.

3. W razie potrzeby zmodyfikuj connection string w pliku `./MiniHelpdesk/appsettings.json`.

4. Przywróć zależności projektu:

```bash
dotnet restore
```

5. Skompiluj rozwiązanie:

```bash
dotnet build
```

6. Uruchom aplikację:

```bash
dotnet run --project MiniHelpdesk
```

7. Otwórz przeglądarkę i przejdź pod adres wyświetlony przez aplikację (domyślnie `https://localhost:7237`).

---

## Uruchomienie testów

Testy można uruchomić poleceniem:

```bash
dotnet test
```

---

## Użyta baza danych

Projekt wykorzystuje **Microsoft SQL Server 2022**.

Dostęp do bazy danych realizowany jest z wykorzystaniem **ADO.NET** (`SqlConnection`, `SqlCommand` oraz `SqlTransaction`).

---

## Middleware

Własny middleware znajduje się w pliku:

`./MiniHelpdesk/Middleware/RequestTimingMiddleware.cs`

Middleware odpowiada za logowanie:
- metody HTTP,
- ścieżki żądania,
- kodu odpowiedzi,
- czasu obsługi żądania.

Middleware jest rejestrowany w pliku `Program.cs`.

---

## Transakcja

Obsługa transakcji została zaimplementowana w klasie `SqlUnitOfWork`.

Podczas tworzenia zgłoszenia `TicketService` wykonuje zapis zgłoszenia (`Ticket`)
oraz pierwszego komentarza (`TicketComment`) wewnątrz metody
`ExecuteInTransactionAsync()`.

Jeżeli obie operacje zakończą się powodzeniem, wykonywany jest `Commit`.
Jeżeli którakolwiek z nich zakończy się wyjątkiem, wykonywany jest `Rollback`,
dzięki czemu w bazie danych nie pozostają częściowo zapisane dane.

---

## Testy

Testy znajdują się w osobnym projekcie testowym rozwiązania (`MiniHelpdesk.Tests`).

Pozwala sprawdzić logikę biznesową bez uruchamiania całej aplikacji i bez połączenia z bazą danych. Dzięki temu błędy można wykryć szybciej, a zmiany w kodzie są bezpieczniejsze.

---

# Odpowiedzi na pytania

## Dlaczego kolejność middleware w `Program.cs` ma znaczenie?

Każde żądanie przechodzi przez middleware w takiej kolejności, w jakiej zostały zarejestrowane. Jeżeli zmienimy ich kolejność, aplikacja może zachowywać się inaczej. Dobrym przykładem jest obsługa wyjątków lub statycznych plików – źle ustawiony pipeline może powodować nieoczekiwane błędy.

## Czym różni się `app.Use` od `app.Run`?

`app.Use` może wykonać własny kod i przekazać żądanie dalej przez `await next()`. `app.Run` kończy pipeline i sam generuje odpowiedź, więc żadne kolejne middleware nie zostaną już wykonane.

## Dlaczego kontroler nie powinien zawierać całej logiki aplikacji?

Kontroler powinien zajmować się obsługą HTTP i przekazywać żądania do warstwy serwisów. Dzięki temu logika biznesowa jest oddzielona od warstwy prezentacji, kod jest bardziej czytelny i łatwiejszy do testowania oraz rozwijania.

## Co daje test jednostkowy warstwy `Service`?

Pozwala sprawdzić, czy logika biznesowa działa poprawnie niezależnie od kontrolerów i bazy danych. Dzięki temu łatwo wykryć błędy po zmianach w kodzie i mieć pewność, że serwis zachowuje się zgodnie z oczekiwaniami.

## Co powinno się stać, jeśli zapis zgłoszenia się uda, ale zapis komentarza zakończy się błędem?

Cała transakcja powinna zostać wycofana (`Rollback`). W bazie danych nie powinno zostać ani zgłoszenie, ani komentarz, ponieważ obie operacje muszą zakończyć się sukcesem jako jedna całość.
