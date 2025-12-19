namespace Education.Domain.Entities
{
    public abstract class EntityClassInformation
    {
        //* ================= Abstract ===================== *//

        //✅  
        /*  Bu class təkbaşına istifadə olunmaq üçün deyil.
         Bu class-dan birbaşa obyekt yaratmaq olmaz
         Bu class başqaları üçün əsas (şablon) rolunu oynayır
         Information info = new Information(); // ❌ OLMAZ  */


        //* ================= Virtual ===================== *//

        //✅  
        /* Virtual niyə qeyd edirik? Çünki yazılan property indi ehtiyac olmaya bilər sonradan
            gətirə bilək deyə virutal yazılmalıdır.
            Virtual olmasa lazy loading edə bilməz.
            public virtual Teacher? Teacher { get; set; } --Ş Teacher məlumatı indi lazım olmaya bilər.
            Lazım olanda EF onu databazadan özü gətirsin deyə virtual yazılır.*/



        //* ================= Soft Delete ===================== *//

        // ✅  
        /* 
          Məlumatı tam silmirik, sadəcə “silinmiş kimi” işarələyirik.
          Data database-də qalır
          Amma istifadəçi üçün görünmür.
          Şəkli silirsən
          Amma “Recently Deleted” qovluğuna düşür
          ➡ Bax bu soft delete-dir.
         */


        //* ================= [NotMapped] ===================== *//

        // ✅  
        /* 
          [NotMapped] Məlumatı  yəni column databazaya əlavə etməmək üçündür.
          Yalnız kodda istifadə edib daha rahat və daha çox LİNQ yazmamaq üçündür.
         */

        //* ================= [JsonIgnore] ===================== *//

        // ✅  
        /* 
          [JsonIgnore] və ya API-yə göndərilən datada (request)
            bu property görünməsin deyə istifadə olunur.
            F.E --> {
                      "id": 1,
                      "username": "Seyid",
                      "password": "123456"
                    }
            Parol hamıya görsənməli deyil. Bunun üçün istifadə edirik JsonIgnore-u
            result: {
                      "id": 1,
                      "username": "Seyid"
                    }
         */


        //* ================= Token Entity ===================== *//

        // ✅  
        /* 
          1 - Əgər tokenləri MyUser-də saxlasaq köhnə tokenləri itiririk.
          2 - Token entity ilə BÜTÜN token-ları saxlayırıq və bütün token tarixçəsi qalır
          3 - MyUser-də yalnız bir token - Telefon login olsa, kompüter logout olur!
          4 - Token Entity ilə isə 
             4.1. Hər device üçün ayrı token
             4.2. Kompüter: token1
             4.3. Telefon: token2
             4.4. Tablet: token3
             4.5. Hamısı aktiv ola bilər!
          5 - Əgər token-i MyUser-də saxlasaq:
             5.1. Token-i revoke etmək üçün MyUser update etməliyik
             5.2. Token history yoxdur
             5.3. Security audit çətin
          6 - "Refresh", "Access", "ResetPassword", "VerifyEmail" MyUser-də bunu edə bilmərik
         */

        //* ================= property-ni expression-bodied(=>)  ===================== *//

        // ✅  
        /* 
          1 - property-ni expression-bodied(=>) yəni bu cür yazılışdır.
            [NotMapped]
            public int TotalCreditsCompleted =>
            StudentCourses?
                .Where(sc => sc.Status == "Completed" && sc.Grade >= 50)
                .Sum(sc => sc.Course?.Credits ?? 0) ?? 0;

        2 - Aciq sekilde 
            public int TotalCreditsCompleted
            {
                get
                {
                    if (StudentCourses == null)
                    {
                        return 0;
                    }

                    int totalCredits = 0;

                
                    foreach (var sc in StudentCourses)
                    {
                        if (Status == "Completed" && sc.Grade >= 50)
                        {
                            if (sc.Course != null)
                            {
                                totalCredits += sc.Course.Credits;
                            }
                        }
                    }

                    return totalCredits;
                }
            }
         */

        //* =================   ===================== *//

        // ✅  
        /* 
         

         */

    }
}
