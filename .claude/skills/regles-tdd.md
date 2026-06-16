---
name: cvtech-tdd-expert
description: Force l'écriture des tests de spécification avant le code métier
globs: "tests/**/*.cs"
---
# PROTOCOLE TDD
Le projet suit une approche Test-Driven Development (TDD) stricte en utilisant **xUnit** et **FluentAssertions**. Les tests servent de documentation vivante du métier.

# INSTRUCTIONS POUR LA GÉNÉRATION DE CODE

En tant qu'assistant IA, tu DOIS respecter ce cycle de développement :

## 1. CYCLE RED-GREEN-REFACTOR OBLIGATOIRE
*   **Étape 1 (Red)** : Tu dois TOUJOURS écrire le test unitaire en premier. N'écris pas le code de production correspondant tant que le test n'a pas été présenté et validé.
*   **Étape 2 (Green)** : Une fois le test généré, écris le code minimum dans la couche Application ou Domain pour le faire passer.
*   **Étape 3 (Refactor)** : Améliore le code généré tout en gardant le test au vert.

## 2. STRUCTURE ET NOMMAGE DES TESTS
*   Les noms des méthodes de test doivent être rédigés en **français**, de manière très descriptive, pour expliquer la règle métier.
*   **Format attendu** : `[Sujet]_[Condition]_[RésultatAttendu]`
*   *Exemple valide* : `public void UnCandidatBloque_QuandIlTenteDePostuler_AlorsLeSystemeLeveUnePermissionRefuseeException()`
*   *Exemple invalide* : `public void Test_PostulerAnnonce()`

## 3. PATRON AAA (ARRANGE, ACT, ASSERT)
Chaque test doit être visuellement séparé en trois blocs distincts par des commentaires :
```csharp
// Arrange
var candidat = new ProfilCandidat(...);
// Act
Action action = () => candidat.Postuler(...);
// Assert
action.Should().Throw<PermissionRefuseeException>();
```

## 4. FLUENT ASSERTIONS
*   L'utilisation des assertions classiques (`Assert.Equal`, `Assert.True`) est **INTERDITE**.
*   Tu dois utiliser exclusivement la syntaxe FluentAssertions (ex: `resultat.Should().BeTrue()`, `liste.Should().HaveCount(1)`).