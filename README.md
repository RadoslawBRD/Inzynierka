# Projekt inzynierski
Uruchomienie	 

Rozpakować archiwum “BuildSerwer” oraz “BuildClient” do osobnych folderów
Z folderu BuildSerwer uruchomić plik “Projekt Inżynierski Serwer Zombie.exe” (możliwe, że trzeba akceptować reguły zapory Windows) 
Z folderu BuildClient uruchomić pik “Projekt Inżynierski Zombie.exe” (możliwe, że trzeba akceptować reguły zapory Windows) 
 
Dołączenie do serwera 
Jeśli serwer uruchomiony na tym samym komputerze co klient, należy wprowadzić nazwę użytkownika, a następnie przycisk “Connect” 
Aby przywrócić kontrole nad myszką należy kliknąć przycisk “ESC” 
 
Uruchomienie gry 
Wersja produkcyjna  
Gra uruchamia się automatycznie, gdy dołączy 5 osób do serwera 

Wersja deweloperska  
Gra uruchomi się po wpisaniu komendy ‘start_game’ 
 
Użycie komend (wymaga połączenie do serwera) (komendy sa głównie dla celów debugowania) 
Wcisnąć przycisk “ESC” aby uzyskać kontrole nad myszką 
Wcisnąć przycisk tylda “~” 
Kliknąć myszką na pole tekstowe u góry ekranu 
Wpisać komendę 
Kliknąć myszką poza polem tekstowym 
Kliknąć enter na klawiaturze numerycznej 
Kliknąć przycisk tylda “~”, aby zamknąć pole tekstowe 
Kliknąć “ESC” aby wrócić do gry 

Użycie komand zostanie uproszczone w wersji produkcyjnej 

Komendy: 
start_game – uruchamia tryb gry polegający na zabiciu X zombie, aktualnie ustawione na 5, wymagane, aby było 2 połączonych graczy (może być uruchomione na jednym komputerze) 
restart_game – resetuje serwer do podstawowego trybu, czyli brak mastera, brak przeciwników 
kill_all_enemies – dawno nie była używana, może nie zadziałać tak jak chcemy 
add_granade <ilosć> - daje <ilość> granatów graczowi, który wpisał komendę Przykład użycia: add_granade 5 
set_map <nazwa mapy> - zmienia mapę na wybraną (“KillHouseMap“ lub “StadiumMap”) Przykład użycia: set_map KillHouseMap 

Poruszanie w grze: 
WASD – poruszanie 
Spacje – skok 
E - uzupełnienie amunicji (o ile starcza pieniędzy) 
Lewy przycisk myszy - strzał lub przeładowanie, jeśli magazynek jest pusty (stał o ile mamy pociski w magazynku, przeładowanie o ile mamy dodatkową amunicje) 
Prawy przycisk myszy – rzut granatem (o ile mamy granat)
  
# GameServer  
Zawiera pliki serwera, może być uruchamiany przez edytor oraz jako build standalone, działa poprawnie pod windowsem i linuxem.

# GameClient
Zawiera pliki klienta, może byc uruchamiany przez edytor oraz jako build standalone.
