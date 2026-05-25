using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalAI.Infrastructure.Services
{
    /// <summary>
    /// Neo4j Service - Quản lý kết nối và query đến Neo4j database
    /// Knowledge Graph chứa: Disease -> Symptoms -> Risk Factors -> Recommendations
    /// </summary>
    public interface INeo4jService
    {
        Task<List<string>> GetAdviceByRiskLevelAsync(string disease, double riskScore);
        Task<List<string>> GetPreventionTipsAsync(string disease);
        Task<List<string>> GetLifestyleRecommendationsAsync(List<string> riskFactors);
        Task InitializeKnowledgeGraphAsync();
    }

    public class Neo4jService : INeo4jService
    {
        private readonly IAsyncSession _session;
        private readonly IDriver _driver;

        public Neo4jService(string uri, string username, string password)
        {
            _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(username, password));
            _session = _driver.AsyncSession();
        }

        /// <summary>
        /// Lấy lời khuyên dựa trên chỉ số y tế có giá trị cao
        /// Query: Indicator -[:INDICATES_RISK_OF]-> Disease -[:HAS_ADVICE_FOR]-> Advice
        /// </summary>
        public async Task<List<string>> GetAdviceByRiskLevelAsync(string disease, double riskScore)
        {
            try
            {
                // Tìm disease theo tên Vietnamese
                var query = @"
                    MATCH (d:Disease {name: $disease})
                    MATCH (d)-[:HAS_ADVICE_FOR]->(a:Advice)
                    RETURN DISTINCT a.content as advice
                    LIMIT 5
                ";

                var result = await _session.RunAsync(query, new { disease });
                var records = await result.ToListAsync();

                return records.Select(r => r["advice"].As<string>()).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Neo4j Error: {ex.Message}");
                return new List<string>();
            }
        }

        /// <summary>
        /// Lấy các chỉ số liên quan đến bệnh
        /// </summary>
        public async Task<List<string>> GetPreventionTipsAsync(string disease)
        {
            try
            {
                // Tìm các Indicator liên quan tới bệnh
                var query = @"
                    MATCH (i:Indicator)-[:INDICATES_RISK_OF]->(d:Disease {name: $disease})
                    RETURN DISTINCT i.label as indicator, i.name as indicatorName
                    LIMIT 10
                ";

                var result = await _session.RunAsync(query, new { disease });
                var records = await result.ToListAsync();

                return records.Select(r => 
                    $"Kiểm soát {r["indicator"].As<string>()} ({r["indicatorName"].As<string>()})"
                ).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Neo4j Error: {ex.Message}");
                return new List<string>();
            }
        }

        /// <summary>
        /// Lấy các bệnh liên quan (complications)
        /// </summary>
        public async Task<List<string>> GetLifestyleRecommendationsAsync(List<string> riskFactors)
        {
            try
            {
                // Tìm bệnh có complication với bệnh chính
                var query = @"
                    MATCH (d:Disease)-[:COMPLICATION_OF]->(related:Disease)
                    RETURN DISTINCT related.name as relatedDisease
                    LIMIT 10
                ";

                var result = await _session.RunAsync(query);
                var records = await result.ToListAsync();

                return records.Select(r => 
                    $"⚠️ Cảnh báo: Có nguy cơ {r["relatedDisease"].As<string>()}"
                ).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Neo4j Error: {ex.Message}");
                return new List<string>();
            }
        }

        /// <summary>
        /// Khởi tạo Knowledge Graph với dữ liệu cơ bản
        /// </summary>
        public async Task InitializeKnowledgeGraphAsync()
        {
            try
            {
                // Xóa dữ liệu cũ
                await _session.RunAsync("MATCH (n) DETACH DELETE n");

                // Tạo Diseases
                await CreateDiseasesAsync();

                // Tạo Risk Factors
                await CreateRiskFactorsAsync();

                // Tạo Advice
                await CreateAdviceAsync();

                // Tạo Prevention Tips
                await CreatePreventionTipsAsync();

                // Tạo Lifestyle Recommendations
                await CreateLifestyleRecommendationsAsync();

                // Tạo quan hệ
                await CreateRelationshipsAsync();

                Console.WriteLine("Knowledge Graph initialized successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing knowledge graph: {ex.Message}");
            }
        }

        private async Task CreateDiseasesAsync()
        {
            var query = @"
                CREATE 
                (d1:Disease {name: 'Diabetes', description: 'Blood sugar disorder', icd10: 'E11'}),
                (d2:Disease {name: 'Hypertension', description: 'High blood pressure', icd10: 'I10'}),
                (d3:Disease {name: 'Heart Disease', description: 'Cardiovascular disorder', icd10: 'I25'}),
                (d4:Disease {name: 'Obesity', description: 'Excess body weight', icd10: 'E66'}),
                (d5:Disease {name: 'Stroke', description: 'Cerebrovascular accident', icd10: 'I63'})
            ";
            await _session.RunAsync(query);
        }

        private async Task CreateRiskFactorsAsync()
        {
            var query = @"
                CREATE
                (rf1:RiskFactor {name: 'High Blood Glucose', category: 'metabolic'}),
                (rf2:RiskFactor {name: 'High Blood Pressure', category: 'cardiovascular'}),
                (rf3:RiskFactor {name: 'High Cholesterol', category: 'metabolic'}),
                (rf4:RiskFactor {name: 'Smoking', category: 'lifestyle'}),
                (rf5:RiskFactor {name: 'Obesity', category: 'metabolic'}),
                (rf6:RiskFactor {name: 'Sedentary Lifestyle', category: 'lifestyle'}),
                (rf7:RiskFactor {name: 'Alcohol Abuse', category: 'lifestyle'}),
                (rf8:RiskFactor {name: 'High Stress', category: 'psychological'})
            ";
            await _session.RunAsync(query);
        }

        private async Task CreateAdviceAsync()
        {
            var query = @"
                CREATE
                (a1:Advice {content: 'Monitor blood glucose levels regularly (3-6 months)', minRisk: 0.3, maxRisk: 0.6, priority: 1}),
                (a2:Advice {content: 'Consult an endocrinologist for personalized diabetes management', minRisk: 0.6, maxRisk: 1.0, priority: 1}),
                (a3:Advice {content: 'Start a low-sodium diet to control blood pressure', minRisk: 0.3, maxRisk: 1.0, priority: 1}),
                (a4:Advice {content: 'Take antihypertensive medications as prescribed', minRisk: 0.6, maxRisk: 1.0, priority: 1}),
                (a5:Advice {content: 'Get blood lipid panel done quarterly', minRisk: 0.5, maxRisk: 1.0, priority: 2}),
                (a6:Advice {content: 'Consider cardiac screening with your physician', minRisk: 0.7, maxRisk: 1.0, priority: 1}),
                (a7:Advice {content: 'Work on weight loss with a balanced diet', minRisk: 0.4, maxRisk: 1.0, priority: 1}),
                (a8:Advice {content: 'Maintain daily exercise routine for cardiovascular health', minRisk: 0.0, maxRisk: 1.0, priority: 2}),
                (a9:Advice {content: 'Quit smoking to significantly reduce health risks', minRisk: 0.5, maxRisk: 1.0, priority: 1}),
                (a10:Advice {content: 'Practice stress management techniques like meditation', minRisk: 0.3, maxRisk: 1.0, priority: 2})
            ";
            await _session.RunAsync(query);
        }

        private async Task CreatePreventionTipsAsync()
        {
            var query = @"
                CREATE
                (p1:Prevention {content: 'Maintain fasting glucose below 126 mg/dL', priority: 1}),
                (p2:Prevention {content: 'Keep blood pressure below 130/80 mmHg', priority: 1}),
                (p3:Prevention {content: 'Maintain total cholesterol below 200 mg/dL', priority: 1}),
                (p4:Prevention {content: 'Achieve and maintain healthy BMI (18.5-24.9)', priority: 1}),
                (p5:Prevention {content: 'Avoid processed foods and sugary drinks', priority: 2}),
                (p6:Prevention {content: 'Get 7-9 hours of quality sleep daily', priority: 2}),
                (p7:Prevention {content: 'Exercise at least 150 minutes per week', priority: 1}),
                (p8:Prevention {content: 'Reduce salt intake to less than 2.3g/day', priority: 2}),
                (p9:Prevention {content: 'Avoid smoking and secondhand smoke', priority: 1}),
                (p10:Prevention {content: 'Limit alcohol consumption', priority: 2})
            ";
            await _session.RunAsync(query);
        }

        private async Task CreateLifestyleRecommendationsAsync()
        {
            var query = @"
                CREATE
                (lr1:LifestyleRecommendation {content: 'Start with 30 minutes of moderate exercise, 5 days a week'}),
                (lr2:LifestyleRecommendation {content: 'Incorporate more vegetables and whole grains into meals'}),
                (lr3:LifestyleRecommendation {content: 'Reduce sodium intake by avoiding processed foods'}),
                (lr4:LifestyleRecommendation {content: 'Practice yoga or meditation for stress relief'}),
                (lr5:LifestyleRecommendation {content: 'Maintain consistent sleep schedule (10 PM - 6 AM)'}),
                (lr6:LifestyleRecommendation {content: 'Stay hydrated by drinking 8 glasses of water daily'}),
                (lr7:LifestyleRecommendation {content: 'Join a support group for lifestyle changes'}),
                (lr8:LifestyleRecommendation {content: 'Track daily nutrition and activity with a health app'})
            ";
            await _session.RunAsync(query);
        }

        private async Task CreateRelationshipsAsync()
        {
            var query = @"
                // Disease - Advice relationships
                MATCH (d:Disease {name: 'Diabetes'}), (a:Advice) WHERE a.content CONTAINS 'glucose' OR a.content CONTAINS 'endocrinologist'
                CREATE (d)-[:HAS_ADVICE]->(a);

                MATCH (d:Disease {name: 'Hypertension'}), (a:Advice) WHERE a.content CONTAINS 'blood pressure' OR a.content CONTAINS 'antihypertensive'
                CREATE (d)-[:HAS_ADVICE]->(a);

                MATCH (d:Disease {name: 'Heart Disease'}), (a:Advice) WHERE a.content CONTAINS 'cardiac' OR a.content CONTAINS 'lipid'
                CREATE (d)-[:HAS_ADVICE]->(a);

                // Disease - Prevention relationships
                MATCH (d:Disease {name: 'Diabetes'}), (p:Prevention) WHERE p.content CONTAINS 'glucose' OR p.content CONTAINS 'exercise'
                CREATE (d)-[:HAS_PREVENTION]->(p);

                MATCH (d:Disease {name: 'Hypertension'}), (p:Prevention) WHERE p.content CONTAINS 'blood pressure' OR p.content CONTAINS 'salt'
                CREATE (d)-[:HAS_PREVENTION]->(p);

                // RiskFactor - Lifestyle Recommendation relationships
                MATCH (rf:RiskFactor {name: 'Sedentary Lifestyle'}), (lr:LifestyleRecommendation) WHERE lr.content CONTAINS 'exercise'
                CREATE (rf)-[:HAS_LIFESTYLE_RECOMMENDATION]->(lr);

                MATCH (rf:RiskFactor {name: 'Obesity'}), (lr:LifestyleRecommendation) WHERE lr.content CONTAINS 'vegetables' OR lr.content CONTAINS 'exercise'
                CREATE (rf)-[:HAS_LIFESTYLE_RECOMMENDATION]->(lr);

                MATCH (rf:RiskFactor {name: 'High Stress'}), (lr:LifestyleRecommendation) WHERE lr.content CONTAINS 'meditation' OR lr.content CONTAINS 'sleep'
                CREATE (rf)-[:HAS_LIFESTYLE_RECOMMENDATION]->(lr);

                MATCH (rf:RiskFactor {name: 'Smoking'}), (lr:LifestyleRecommendation) WHERE lr.content CONTAINS 'support group'
                CREATE (rf)-[:HAS_LIFESTYLE_RECOMMENDATION]->(lr);
            ";
            await _session.RunAsync(query);
        }

        public async ValueTask DisposeAsync()
        {
            await _session?.CloseAsync();
            await _driver?.CloseAsync();
        }
    }
}
