# MicroService Web C#

## Description

Cardamon est un site web developpé dans le cadre d'un projet d'intégration d'application.

Ce site, developpé grâce au Framework Blazor et ASP.NET du C#, permet la gestion de collections de cartes Pokémon, par utilisateurs,
et leurs partages si autorisation. La gestion des données se base sur ce qu'il se fait de mieux en matière de sécurité et de protection des données.

La base de donnée des cartes Pokémon a été généré par un script Python (PokéService/Migration/pkdump.py), dumpant et filtrant celle exposé par l'API de [TCGdex](https://tcgdex.dev).
Ce dump extrait les données (hors images) d'environ 15 000 cartes en version Française, en un peu plus d'1h.

## Guide des pages

### Authorized

#### Collection

  Si vous êtes connecté, vous trouverez ici votre collection sous la forme d'une liste de carte que vous y avez ajouté. Vous pourrez rechercher une carte particulière grâce au champs se trouvant en haut ou supprimer une carte en cliquant sur le bouton associé. Il est également possible de cliquer sur la carte pour en avoir un affichage détaillé depuis lequel vous pourrez aussi l'ajouter ou la supprimer de votre collection personnelle.
  
#### Mon Compte

  Depuis cette page, il vous sera possible de supprimer votre compte définitivement ou de récupérer vos identifiants afin de pouvoir le partager à vos amis.
  
### Not Authorized

##### Connexion

  Comme son nom l'indique, cette page vous permettra de vous connecter à conditions que vos identifiants et mod de passe soient valides.
  
#### Inscription

  L'inscription sera possible depuis cette page avec une adresse Mail conforme et pas encore enregistrée, un nom d'utilisateur unique composé de caractères alphanumériques et un mot de passe également composé de caractères alphanumériques. Chacun de ces champs ne peut pas être vides.
  
### Les deux

##### Recherche

  Cette page vous permettra de rechercher une carte dans la base de donnée afin de pouvoir la voir de manière plus détaillée. Il sera possible de l'ajouter à votre collection depuis cette page en cliquant sur le + si vous êtes connecté. Sinon, vous pourrez seulement afficher les détails de la carte en cliquant dessus.
  
#### Collections

  Cette page cous permettra de consulter la collection d'un ami ou la votre si vous n'êtes pas connecté. Elle ne donne évidemment pas la possibilité de modifié les collections affichées. La recherche est également disponible dans cette page.

### Remerciement
- [TCGdex](https://tcgdex.dev) pour toutes les données des cartes utilisées

### Credit
Guillaume ASSAILLY
Romain KLODZINSKI
Paul QUINZI
Tous droits réservés (c) 2024

### Licence MIT
