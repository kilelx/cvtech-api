using CVTech.Modules.GestionIdentite.Contracts;
using CVTech.Modules.GestionIdentite.Domain.Enums;

namespace CVTech.Modules.GestionIdentite.Domain.Services;

public static class MatricePermissions
{
    private static readonly Dictionary<ActionSecurisee, HashSet<RoleUtilisateur>> _matrice = new()
    {
        [ActionSecurisee.ConsulterAnnonce]               = [RoleUtilisateur.Candidat, RoleUtilisateur.Entreprise, RoleUtilisateur.Administrateur],
        [ActionSecurisee.ConsulterFluxRSS]               = [RoleUtilisateur.Candidat, RoleUtilisateur.Entreprise, RoleUtilisateur.Administrateur],
        [ActionSecurisee.ModifierCV]                     = [RoleUtilisateur.Candidat, RoleUtilisateur.Administrateur],
        [ActionSecurisee.PostulerAnnonce]                = [RoleUtilisateur.Candidat, RoleUtilisateur.Administrateur],
        [ActionSecurisee.SoumettrePropositionFreelance]  = [RoleUtilisateur.Candidat, RoleUtilisateur.Administrateur],
        [ActionSecurisee.PublierAnnonceEmploi]           = [RoleUtilisateur.Entreprise, RoleUtilisateur.Administrateur],
        [ActionSecurisee.PublierAppelOffre]              = [RoleUtilisateur.Entreprise, RoleUtilisateur.Administrateur],
        [ActionSecurisee.ConsulterCandidaturesRecues]    = [RoleUtilisateur.Entreprise, RoleUtilisateur.Administrateur],
        [ActionSecurisee.SAbonnerDomaineMetier]          = [RoleUtilisateur.Candidat, RoleUtilisateur.Entreprise, RoleUtilisateur.Administrateur],
        [ActionSecurisee.PublierArticleActualite]        = [RoleUtilisateur.Administrateur],
        [ActionSecurisee.ModererContenu]                 = [RoleUtilisateur.Administrateur],
        [ActionSecurisee.BloquerCompte]                  = [RoleUtilisateur.Administrateur],
        [ActionSecurisee.GererDomainesMetier]            = [RoleUtilisateur.Administrateur],
    };

    public static bool EstAutorise(RoleUtilisateur role, ActionSecurisee action)
    {
        return _matrice.TryGetValue(action, out var rolesAutorises) && rolesAutorises.Contains(role);
    }
}
