using System.Collections.Generic;
using System.Text;
using SkalluUtils.Extensions;
using UnityEngine;

public class LSystem : MonoBehaviour
{
    #region INSPECTOR FIELDS
    [SerializeField] private GameObject branchPrefab;
    [SerializeField] private GameObject leafPrefab;
    
    [Space]
    [SerializeField, Range(1, 10)] public int iterations = 5;
    [SerializeField, Range(1, 10)] public int length = 5;
    [SerializeField, Range(1, 90)] public int angle = 30;
    [SerializeField, Range(0.01f, 1f)] public float width = 1;
    #endregion

    #region BRANCH MODIFIERS
    private class BranchSize
    {
        private readonly Vector3 branchScale; // (50x50x50 : 150x150x150)
        public Vector3 BranchScale => branchScale;
        
        private readonly float moveMultiplier; // (1 : 2.8)
        public float MoveMultiplier => moveMultiplier;

        public BranchSize(Vector3 branchScale, float moveMultiplier)
        {
            this.branchScale = branchScale;
            this.moveMultiplier = moveMultiplier;
        }
    }
    
    private BranchSize branchSize;
    
    private float branchTopWidth = 53;
    private float branchBottomWidth = 50;
    #endregion

    #region TURTLE
    private class TurtleData
    {
        internal Vector3 position;
        internal Quaternion rotation;
        internal float topWidth, bottomWidth = 50;
    }
    
    private Stack<TurtleData> turtleDataStack;
    #endregion

    #region RULE STUFF
    private Dictionary<char, string> treeRules;
    private RulePicker rulePicker;
    private string ruleString;
    #endregion

    private MeshCombiner meshCombiner;

    // step by step generation
    // private int idx = 0;
    // private bool done = false;

    private void Awake()
    {
        rulePicker = new RulePicker();
        meshCombiner = GetComponent<MeshCombiner>();
    }

    private void Start()
    {
        treeRules = new Dictionary<char, string>
        {
            {'X', ""},
            {'F', "FF"}
        };

        turtleDataStack = new Stack<TurtleData>();
    }

    /// <summary>
    /// Main method
    /// </summary>
    public void Generate()
    {
        branchSize = length switch
        {
            1 => new BranchSize(new Vector3(50, 50, 50), 1),
            2 => new BranchSize(new Vector3(60, 60, 60), 1.2f),
            3 => new BranchSize(new Vector3(70, 70, 70), 1.4f),
            4 => new BranchSize(new Vector3(80, 80, 80), 1.6f),
            5 => new BranchSize(new Vector3(90, 90, 90), 1.8f),
            6 => new BranchSize(new Vector3(100, 100, 100), 2f),
            7 => new BranchSize(new Vector3(110, 110, 110), 2.2f),
            8 => new BranchSize(new Vector3(120, 120, 120), 2.4f),
            9 => new BranchSize(new Vector3(130, 130, 130), 2.6f),
            10 => new BranchSize(new Vector3(140, 140, 140), 2.8f),
            _ => new BranchSize(new Vector3(50, 50, 50), 1)
        };

        Clear();
        GenerateRuleString();
        BuildTree();
    }

    /// <summary>
    /// Clears generated tree
    /// </summary>
    private void Clear() => GetComponent<MeshFilter>().mesh = null;

    /// <summary>
    /// Generates rule string based on iterations
    /// </summary>
    private void GenerateRuleString()
    {
        treeRules['X'] = rulePicker.PickRandomRule();

        if (treeRules['X'] != null && treeRules['X'].Length > 0)
        {
            ruleString = "X";
            var sb = new StringBuilder();
    
            for (int i = 0; i < iterations; i++)
            {
                foreach (var c in ruleString)
                {
                    sb.Append(treeRules.ContainsKey(c) ? treeRules[c] : c.ToString());
                }

                ruleString = sb.ToString();
                sb.Clear();
            }
        }
    }

    /// <summary>
    /// Builds tree iteratively based on generated rule string
    /// </summary>
    private void BuildTree()
    {
        var turtle = Instantiate(new GameObject("Turtle"), transform.position, transform.rotation, transform);

        foreach (var axiom in ruleString)
        {
            switch (axiom)
            {
                case 'F': // adds branch or leaf
                {
                    var branch = Instantiate(branchPrefab, turtle.transform.position, turtle.transform.rotation, gameObject.transform);
                    branch.transform.localScale = branchSize.BranchScale;

                    // set branch top and bottom width
                    var branchSkinnedMeshRenderer = branch.GetComponent<SkinnedMeshRenderer>();
                    branchSkinnedMeshRenderer.SetBlendShapeWeight(0, branchTopWidth);
                    branchSkinnedMeshRenderer.SetBlendShapeWeight(1, branchBottomWidth);

                    branchTopWidth += 3 * width;
                    branchBottomWidth += 3 * width;
                    
                    // convert skinned mesh renderer to default mesh renderer
                    var mesh = new Mesh();
                    branchSkinnedMeshRenderer.BakeMesh(mesh);
                    branch.GetComponent<MeshFilter>().mesh = mesh;
                    branch.AddComponent<MeshRenderer>().materials = branchSkinnedMeshRenderer.materials;
                    Destroy(branchSkinnedMeshRenderer);

                    // combine meshes then destroy unused child objects
                    meshCombiner.CombineMeshes();
                    meshCombiner.ClearUnusedObjects();
                    
                    turtle.transform.Translate(Vector3.up * branchSize.MoveMultiplier);

                    break;
                }

                case 'X': // generates 'F's
                {
                    break;
                }

                case '-': // rotates turtle forward
                {
                    turtle.transform.Rotate(Vector3.forward * angle);
                    break;
                }

                case '+': // rotates turtle backwards
                {
                    turtle.transform.Rotate(Vector3.back * angle);
                    break;
                }

                case '*': // rotates turtle up
                {
                    turtle.transform.Rotate(Vector3.up * angle);
                    break;
                }

                case '/': // rotates turtle down
                {
                    turtle.transform.Rotate(Vector3.down * angle);
                    break;
                }

                case '[': // saves current turtle data
                {
                    turtleDataStack.Push(new TurtleData
                    {
                        position = turtle.transform.position,
                        rotation = turtle.transform.rotation,
                        topWidth = branchTopWidth,
                        bottomWidth = branchBottomWidth
                    });

                    break;
                }

                case ']': // loads previously saved turtle data
                {
                    var restoredPositionRotationData = turtleDataStack.Pop();
                    turtle.transform.position = restoredPositionRotationData.position;
                    turtle.transform.rotation = restoredPositionRotationData.rotation;
                    branchTopWidth = restoredPositionRotationData.topWidth;
                    branchBottomWidth = restoredPositionRotationData.bottomWidth;

                    break;
                }

                default:
                {
                    Debug.Log("Invalid L-System axiom occured!".Color(Color.red));
                    break;
                }
            }
        }
        
        Destroy(turtle);
    }

    // private IEnumerator ConstructStepByStep()
    // {
    //     if (!done)
    //     {
    //         while (true)
    //         {
    //             if (idx < ruleString.Length)
    //             {
    //                 if (ruleString[idx] == 'F')
    //                 {
    //                     yield return new WaitForSeconds(0.05f);
    //                 }
    //             }
    //             else
    //                 break;
    //
    //             BuildTree();
    //         }
    //     }
    //     else
    //     {
    //         yield return null;
    //     }
    // }
    //
    //
    //
    // private void BuildTree()
    // {
    //     if (idx < ruleString.Length)
    //     {
    //         switch (ruleString[idx])
    //         {
    //             // draws straight line
    //             case 'F':
    //             {
    //                 initialPosition = transform.position;
    //                 transform.Translate(Vector3.up * length);
    //
    //                 var branchInstance = Instantiate(branchPrefab);
    //
    //                 if (!branchPrefab.TryGetComponent(out LineRenderer lineRenderer))
    //                 {
    //                     Debug.Log("Spawned branch has no Line Renderer! New Line Renderer component has been attached."
    //                         .Color(Color.red));
    //                     lineRenderer = branchInstance.AddComponent<LineRenderer>();
    //                 }
    //
    //                 lineRenderer.SetPosition(0, initialPosition);
    //                 lineRenderer.SetPosition(1, transform.position);
    //
    //                 break;
    //             }
    //
    //             // generates 'F's
    //             case 'X':
    //             {
    //                 break;
    //             }
    //
    //             // rotates tree spawner clockwise
    //             case '-':
    //             {
    //                 transform.Rotate(Vector3.forward * angle);
    //                 break;
    //             }
    //
    //             // rotates tree spawner anti-clockwise
    //             case '+':
    //             {
    //                 transform.Rotate(Vector3.back * angle);
    //                 break;
    //             }
    //
    //             // rotates tree 
    //             case '*':
    //             {
    //                 transform.Rotate(Vector3.up * angle);
    //                 break;
    //             }
    //
    //             // rotates tree
    //             case '/':
    //             {
    //                 transform.Rotate(Vector3.down * angle);
    //                 break;
    //             }
    //
    //             // saves current transform
    //             case '[':
    //             {
    //                 turtleDataStack.Push(new TurtleData
    //                 {
    //                     position = transform.position,
    //                     rotation = transform.rotation
    //                 });
    //
    //                 break;
    //             }
    //
    //             // restores to previously saved transform
    //             case ']':
    //             {
    //                 var restoredPositionRotationData = turtleDataStack.Pop();
    //                 transform.position = restoredPositionRotationData.position;
    //                 transform.rotation = restoredPositionRotationData.rotation;
    //
    //                 break;
    //             }
    //
    //             default:
    //             {
    //                 Debug.Log("Invalid L-System axiom occured!".Color(Color.red));
    //                 break;
    //             }
    //         }
    //
    //         idx += 1;
    //     }
    //     else
    //     {
    //         done = true;
    //     }
    // }

    
    
}