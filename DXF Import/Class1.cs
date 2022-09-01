using ABB.Robotics.Math;
using ABB.Robotics.RobotStudio;
using ABB.Robotics.RobotStudio.Environment;
using ABB.Robotics.RobotStudio.Stations;
using System;
using System.Collections.Generic;
using System.Text;

namespace DXF_Import
{
    public class Class1
    {
        // This is the entry point which will be called when the Add-in is loaded
        static CommandBarButton btnCreateModel;
        static CommandBarButton btnCreatePath;
        static CommandBarComboBox cmbIOList;
        static CommandBarComboBox cmbPierceTime;
        static CommandBarComboBox cmbSpeed;
        static CommandBarComboBox cmbKerf;
        static CommandBarComboBox cmbOffset;
        //static CommandBarButton btnStart;
        //static CommandBarButton btnCreatePath;

        public static void AddinMain()
        {
            Logger.AddMessage(new LogMessage("DXFImporter was succesfully added"));


            // Handle events from controls defined in CommandBarControls.xml
            RegisterCommand("DXF_Import.StartButton");
            RegisterCommand("DXF_Import.CloseButton");
            RegisterCommand("DXF_Import.CreateModel");
            RegisterCommand("DXF_Import.CreatePath");


            cmbIOList = CommandBarComboBox.FromID("DXF_Import.ActiveIOs");
            cmbPierceTime = CommandBarComboBox.FromID("DXF_Import.PeirceTime");
            cmbSpeed = CommandBarComboBox.FromID("DXF_Import.CutSpeed");
            cmbKerf = CommandBarComboBox.FromID("DXF_Import.Kerf");
            cmbOffset = CommandBarComboBox.FromID("DXF_Import.Offset");
            btnCreateModel = CommandBarButton.FromID("DXF_Import.CreateModel");
            btnCreatePath = CommandBarButton.FromID("DXF_Import.CreatePath");

            SetupPiececmb();
            System.Threading.Thread threaderIOLoader = new System.Threading.Thread(ComboBoxLoader);
            threaderIOLoader.Start();
            //ComboBoxLoader();

            ABB.Robotics.RobotStudio.Controllers.ControllerManager.SelectedControllerObjectChanged += LoadIOs;
        }

        #region UIFunctions
        static void RegisterCommand(string id)
        {
            var button = CommandBarButton.FromID(id);
            button.UpdateCommandUI += button_UpdateCommandUI;
            button.ExecuteCommand += button_ExecuteCommand;
        }

        static void LoadIOs(object sender, EventArgs e)
        {
            System.Threading.Thread threaderIOLoader = new System.Threading.Thread(ComboBoxLoader);

            threaderIOLoader.Start();
        }


        static void SetupPiececmb()
        {
            cmbPierceTime.Items.Add(new CommandBarComboBoxItem((.1).ToString()));
            for (double i = .5; i < 3; i += .5)
            {
                cmbPierceTime.Items.Add(new CommandBarComboBoxItem(i.ToString()));
            }
            cmbPierceTime.SelectedIndex = 2;
            cmbPierceTime.Enabled = true;
        }

        static void ComboBoxLoader()
        {
            try
            {
                Station station = Station.ActiveStation;



                ABB.Robotics.Controllers.Controller ActiveController = ABB.Robotics.Controllers.Controller.Connect(
                    ABB.Robotics.RobotStudio.Controllers.ControllerManager.SelectedControllerObject.SystemId, ABB.Robotics.Controllers.ConnectionType.RobotStudio);

                if (ActiveController != null)
                {
                    ABB.Robotics.Controllers.IOSystemDomain.SignalCollection signals = ActiveController.IOSystem.GetSignals(ABB.Robotics.Controllers.IOSystemDomain.IOFilterTypes.Output);
                    cmbIOList.Items.Clear();
                    cmbIOList.Enabled = true;
                    cmbIOList.Visible = true;
                    foreach (ABB.Robotics.Controllers.IOSystemDomain.Signal IOitem in signals)
                    {

                        if (IOitem.Type == ABB.Robotics.Controllers.IOSystemDomain.SignalType.DigitalOutput)
                        {
                            CommandBarComboBoxItem TempItem = new CommandBarComboBoxItem(IOitem.Name);
                            cmbIOList.Items.Add(TempItem);
                            if (TempItem.Text.ToLower().Contains("plasma")) cmbIOList.SelectedItem = TempItem;
                        }

                    }
                }


                if (cmbSpeed.SelectedIndex == -1)
                {
                    cmbSpeed.Items.Clear();
                    cmbSpeed.Enabled = true;
                    cmbSpeed.Visible = true;

                    cmbSpeed.Items.Add(new CommandBarComboBoxItem("V5"));
                    cmbSpeed.Items.Add(new CommandBarComboBoxItem("V10"));
                    cmbSpeed.Items.Add(new CommandBarComboBoxItem("V20"));
                    cmbSpeed.Items.Add(new CommandBarComboBoxItem("V30"));
                    cmbSpeed.Items.Add(new CommandBarComboBoxItem("V40"));
                    cmbSpeed.Items.Add(new CommandBarComboBoxItem("V50"));
                    cmbSpeed.Items.Add(new CommandBarComboBoxItem("V100"));
                    cmbSpeed.Items.Add(new CommandBarComboBoxItem("V200"));
                    cmbSpeed.SelectedIndex = 2;
                }


                if (cmbKerf.SelectedIndex == -1)
                {
                    cmbKerf.Items.Clear();
                    cmbKerf.Enabled = true;
                    cmbKerf.Visible = true;
                    cmbKerf.Items.Add(new CommandBarComboBoxItem("0"));
                    cmbKerf.Items.Add(new CommandBarComboBoxItem(".25"));
                    cmbKerf.Items.Add(new CommandBarComboBoxItem(".5"));
                    cmbKerf.Items.Add(new CommandBarComboBoxItem("1"));
                    cmbKerf.Items.Add(new CommandBarComboBoxItem("2"));
                    cmbKerf.SelectedIndex = 2;
                }


                if (cmbOffset.SelectedIndex == -1)
                {
                    cmbOffset.Items.Clear();
                    cmbOffset.Enabled = true;
                    cmbOffset.Visible = true;
                    cmbOffset.Items.Add(new CommandBarComboBoxItem("0"));
                    cmbOffset.Items.Add(new CommandBarComboBoxItem(".1"));
                    cmbOffset.Items.Add(new CommandBarComboBoxItem(".5"));
                    cmbOffset.Items.Add(new CommandBarComboBoxItem("1"));
                    cmbOffset.Items.Add(new CommandBarComboBoxItem("2"));
                    cmbOffset.Items.Add(new CommandBarComboBoxItem("3"));
                    cmbOffset.Items.Add(new CommandBarComboBoxItem("4"));
                    cmbOffset.Items.Add(new CommandBarComboBoxItem("5"));
                    cmbOffset.Items.Add(new CommandBarComboBoxItem("6"));
                    cmbOffset.Items.Add(new CommandBarComboBoxItem("10"));
                    cmbOffset.SelectedIndex = 5;
                }

                }
            catch (Exception)
            {

                return;
            }
        }

        static void button_UpdateCommandUI(object sender, UpdateCommandUIEventArgs e)
        {
            switch (e.Id)
            {
                case "DXF_Import.CreateModel":
                    e.Enabled = true;
                    break;
                case "DXF_Import.CreatePath":
                    e.Enabled = true;
                    break;
                case "DXF_Import.CloseButton":
                    e.Enabled = true;
                    break;
            }
        }


        static void button_ExecuteCommand(object sender, ExecuteCommandEventArgs e)
        {
            switch (e.Id)
            {
                case "DXF_Import.StartButton":
                    StartButtonAction();
                    break;

                case "DXF_Import.CloseButton":
                    CloseButtonAction();
                    break;

                case "DXF_Import.CreateModel":
                    CreateLayout();
                    break;

                case "DXF_Import.CreatePath":
                    CreatePath();
                    break;
            }
        }

        static void StartButtonAction()
        {
            RibbonTab ribbonTab = UIEnvironment.RibbonTabs["DXF_Import.ManufacturingTab"];
            if (Station.ActiveStation == null)
            {
                Logger.AddMessage(new LogMessage("A station must be opened"));
                ribbonTab.Visible = false;
            }
            else
            {
                ribbonTab.Visible = true;
                UIEnvironment.ActiveRibbonTab = ribbonTab;

                UIEnvironment.Windows["ObjectBrowser"].SetTabVisibility(ribbonTab, true);
                UIEnvironment.Windows["ElementBrowser"].SetTabVisibility(ribbonTab, true);

            }
        }

        static void CloseButtonAction()
        {
            RibbonTab ribbonTab = UIEnvironment.RibbonTabs["DXF_Import.ManufacturingTab"];
            ribbonTab.Visible = false;
        }

        #endregion


        #region DXFRegion

        static void SortWires(ref List<Wire> OGWire)
        {

            //Logger.AddMessage(new LogMessage(OGWire.Count.ToString()));
            for (int Looper = 0; Looper < OGWire.Count; Looper++)
            {
                for (int i = 0; i < OGWire.Count; i++)
                {
                    //Logger.AddMessage(new LogMessage("PreSort: "+OGWire[i].EndVertex.Position.ToString(3, ",")));
                    for (int index = i + 2; index < OGWire.Count; index++)
                    {

                        //Logger.AddMessage(new LogMessage(OGWire[i].EndVertex.Position.ToString(3, ",")));
                        //Logger.AddMessage(new LogMessage(OGWire[index].StartVertex.Position.ToString(3, ",")));
                        if (OGWire[i].EndVertex.Position.ToString(3, ",") == OGWire[index].StartVertex.Position.ToString(3, ","))
                        {
                            //System.Windows.Forms.MessageBox.Show(OGWire[i].EndVertex.Position.ToString(3,","));
                            OGWire.Insert(i + 1, OGWire[index]);
                            OGWire.RemoveAt(index + 1);

                        }
                        else if (OGWire[i].EndVertex.Position.ToString(3, ",") == OGWire[index].EndVertex.Position.ToString(3, ","))
                        {
                            OGWire[index].ReverseDirection();
                            //Logger.AddMessage(new LogMessage(OGWire[i].EndVertex.Position.ToString(3, ",") + " and " + OGWire[index].StartVertex.Position.ToString(3, ",")));
                            OGWire.Insert(i + 1, OGWire[index]);
                            OGWire.RemoveAt(index + 1);
                        }
                        else if (OGWire[i].StartVertex.Position.ToString(3, ",") == OGWire[index].StartVertex.Position.ToString(3, ","))
                        {
                            OGWire[i].ReverseDirection();
                            //Logger.AddMessage(new LogMessage(OGWire[i].EndVertex.Position.ToString(3, ",") + " and " + OGWire[index].StartVertex.Position.ToString(3, ",")));

                            OGWire.Insert(i + 1, OGWire[index]);
                            OGWire.RemoveAt(index + 1);
                        }
                    }
                }
            }

        }



        static void LoadDXF(ref List<Wire> DXFAssembly, string FilePath)
        {
            Vector3 Startpoint;
            Vector3 Endpoint;

            double Scalar = 25.4 / 1000;
            Matrix4 CircleMat;
            netDxf.DxfDocument dxfFile = netDxf.DxfDocument.Load(FilePath);

            foreach (netDxf.Entities.Line item in dxfFile.Lines)
            {
                Startpoint = new Vector3(item.StartPoint.X, item.StartPoint.Y, item.StartPoint.Z) * Scalar;
                Endpoint = new Vector3(item.EndPoint.X, item.EndPoint.Y, item.EndPoint.Z) * Scalar;
                Startpoint.x = Math.Round(Startpoint.x, 5);
                Startpoint.y = Math.Round(Startpoint.y, 5);
                Endpoint.x = Math.Round(Endpoint.x, 5);
                Endpoint.y = Math.Round(Endpoint.y, 5);

                DXFAssembly.Add(Body.CreateLine(Startpoint, Endpoint).Wires[0]);
                //DXFDrawing.Bodies.Add(Body.CreateLine(new Vector3(item.StartPoint.X, item.StartPoint.Y, item.StartPoint.Z), new Vector3(item.EndPoint.X, item.EndPoint.Y, item.EndPoint.Z)));
            }

            foreach (netDxf.Entities.Arc item in dxfFile.Arcs)
            {
                netDxf.Entities.LwPolyline TempPolyLine;
                //Startpoint = new Vector3(item.Radius * Math.Cos(item.StartAngle / 180 * Math.PI) + item.Center.X, item.Radius * Math.Sin(item.StartAngle / 180 * Math.PI) + item.Center.Y, 0) * Scalar;
                Vector3 ViaPoint;
                //Endpoint = new Vector3(item.Radius * Math.Cos(item.EndAngle / 180 * Math.PI) + item.Center.X, item.Radius * Math.Sin(item.EndAngle / 180 * Math.PI) + item.Center.Y, 0) * Scalar;
                TempPolyLine = item.ToPolyline(3);

                Endpoint = new Vector3(TempPolyLine.Vertexes[TempPolyLine.Vertexes.Count - 1].Position.X,
                    TempPolyLine.Vertexes[TempPolyLine.Vertexes.Count - 1].Position.Y, 0) * Scalar;

                ViaPoint = new Vector3(TempPolyLine.Vertexes[1].Position.X,
                TempPolyLine.Vertexes[1].Position.Y, 0) * Scalar;

                Startpoint = new Vector3(TempPolyLine.Vertexes[0].Position.X,
                   TempPolyLine.Vertexes[0].Position.Y, 0) * Scalar;

                Startpoint.x = Math.Round(Startpoint.x, 5);
                Startpoint.y = Math.Round(Startpoint.y, 5);
                Endpoint.x = Math.Round(Endpoint.x, 5);
                Endpoint.y = Math.Round(Endpoint.y, 5);
                ViaPoint.x = Math.Round(ViaPoint.x, 5);
                ViaPoint.y = Math.Round(ViaPoint.y, 5);

                DXFAssembly.Add(Body.CreateArc(Endpoint, Startpoint, ViaPoint).Wires[0]);
                //Logger.AddMessage(new LogMessage(DXFAssembly[DXFAssembly.Count-1].Body.Wires[0].StartVertex.Position.ToString()));
                //DXFDrawing.Bodies.Add(Body.CreateArc(Startpoint, Endpoint, ViaPoint));
            }

            foreach (netDxf.Entities.Circle item in dxfFile.Circles)
            {
                CircleMat = new Matrix4(new Vector3(item.Center.X, item.Center.Y, item.Center.Z) * Scalar);

                DXFAssembly.Add(Body.CreateCircle(CircleMat, item.Radius * Scalar).Shells[0].Wires[0]);
                //DXFDrawing.Bodies.Add(Body.CreateCircle(CircleMat, item.Radius));
            }
        }


        #endregion


        #region Robotstudio
        static void PathGenerator(ref Part DXFDrawing, ref RsPathProcedure PlasmaProgram, double Kerf, double VerticalOffset)
        {
            int StartPlasma= 2;
            Station station = Station.ActiveStation;

            RsActionInstruction NextInstruction = new RsActionInstruction(station.ActiveTask, "ConfL", "Off");
            PlasmaProgram.Instructions.Add(NextInstruction);

            NextInstruction = new RsActionInstruction(station.ActiveTask, "ConfJ", "Off");
            PlasmaProgram.Instructions.Add(NextInstruction);


            foreach (Body Bodyitem in DXFDrawing.Bodies)
            {
                foreach (Wire WireItem in Bodyitem.Wires)
                {
                    List<RsTarget> PathTargets = new List<RsTarget>();
                    List<Matrix4> PathPoses = new List<Matrix4>();
                    List<RsPointType> PathMoveTypes = new List<RsPointType>();
                    RsRobTarget PlasmaTarget = new RsRobTarget();
                    RobotStudio.API.Internal.PathGenerator AutoPath = new RobotStudio.API.Internal.PathGenerator();
                    AutoPath.ReferenceSurface = Body.CreateSurfaceRectangle(new Matrix4(new Vector3(-5, -5, 0)), 10, 10).Faces[0];

                    AutoPath.UseReferenceSurface = true;
                    AutoPath.Circular = true;
                    AutoPath.MinDistance = .0001;


                    AutoPath.AddWire(WireItem);
                    AutoPath.GenerateMatrices();
                    PathPoses = AutoPath.GetMatrices();
                    PathMoveTypes = AutoPath.GetMatrixTypeInPath();
                    AutoPath.ClearPreview();
                    AutoPath.Clear();


                    PathPoses.Insert(0, PathPoses[0]);
                    PathPoses[0] = PathPoses[0].Multiply(new Matrix4(new Vector3(0, 0, -.1)));
                    PathMoveTypes.Insert(0, RsPointType.ToPoint);

                    PathPoses.Insert(1, PathPoses[1]);
                    PathPoses[1] = PathPoses[1].Multiply(new Matrix4(new Vector3(Kerf*2, 0, -.01)));
                    PathMoveTypes.Insert(1, RsPointType.ToPoint);

                    PathPoses.Insert(2, PathPoses[2]);
                    PathPoses[2] = PathPoses[2].Multiply(new Matrix4(new Vector3(Kerf * 2, 0, -VerticalOffset)));
                    PathMoveTypes.Insert(2, RsPointType.ToPoint);



                    PathPoses.Add(PathPoses[PathPoses.Count - 1]);
                    PathPoses[PathPoses.Count - 1] = PathPoses[PathPoses.Count - 1].Multiply(new Matrix4(new Vector3(0, 0, -.1)));
                    PathMoveTypes.Add(RsPointType.ToPoint);

                    //System.Windows.Forms.MessageBox.Show(PathPoses.Count.ToString());

                    for (int i = 0; i < PathPoses.Count; i++)
                    {
                        RsRobTarget TempTarget = new RsRobTarget();
                        ExternalAxisValues TempExternal = new ExternalAxisValues();


                        TempExternal.Eax_a = 0;
                        TempExternal.Eax_b = 0;
                        TempExternal.Eax_c = 0;
                        TempExternal.Eax_d = 0;
                        TempExternal.Eax_e = 0;
                        TempExternal.Eax_f = 0;
                        TempTarget.IsInline = true;
                        TempTarget.Name = station.ActiveTask.GetValidRapidName("Plasma", "_", 1);
                        TempTarget.Frame.Matrix = PathPoses[i];
                        TempTarget.Frame.Matrix = TempTarget.Frame.Matrix.Multiply(new Matrix4(new Vector3(Kerf, 0, 0)));
                        TempTarget.Frame.RZ = (90 / 180) * Math.PI;
                        TempTarget.SetExternalAxes(TempExternal, false);
                        //TempTarget.Frame.RZ = 90;

                        station.ActiveTask.DataDeclarations.Add(TempTarget);

                        RsTarget TempRSTarget = new RsTarget(station.ActiveTask.ActiveWorkObject, TempTarget);
                        TempRSTarget.Name = TempTarget.Name;
                        TempRSTarget.WorkObject = station.ActiveTask.ActiveWorkObject;
                        TempRSTarget.Visible = false;
                        station.ActiveTask.Targets.Add(TempRSTarget);

                        PathTargets.Add(TempRSTarget);
                    }
                    RsInstructionArgument InstructionToModify;

                    //RsMoveInstruction NextMoveInstruction;
                    PlasmaProgram.Instructions.Add(CreateNewMove(station.ActiveTask, MotionType.Linear, PathTargets[0].Name, "v1000", "z0"));


                    PlasmaProgram.Instructions.Add(CreateNewMove(station.ActiveTask, MotionType.Linear, PathTargets[1].Name, "v1000", "z0"));


                    NextInstruction = new RsActionInstruction(station.ActiveTask, "WaitTime", "Default");
                    if (NextInstruction.InstructionArguments.TryGetInstructionArgument("Time", out InstructionToModify)) InstructionToModify.Value = ".1";
                    if (NextInstruction.InstructionArguments[0].Name.ToLower().Contains("pos")) NextInstruction.InstructionArguments[0].Enabled = true;
                    PlasmaProgram.Instructions.Add(NextInstruction);



                    NextInstruction = new RsActionInstruction(station.ActiveTask, "Set", "Default");
                    if (NextInstruction.InstructionArguments.TryGetInstructionArgument("Signal", out InstructionToModify)) InstructionToModify.Value = cmbIOList.SelectedItem.Text;
                    PlasmaProgram.Instructions.Add(NextInstruction);

                    
                    PlasmaProgram.Instructions.Add(CreateNewMove(station.ActiveTask, MotionType.Linear, PathTargets[2].Name, cmbSpeed.SelectedItem.Text, "z0"));




                    NextInstruction = new RsActionInstruction(station.ActiveTask, "WaitTime", "Default");
                    if (NextInstruction.InstructionArguments.TryGetInstructionArgument("Time", out InstructionToModify)) InstructionToModify.Value = cmbPierceTime.SelectedItem.Text;
                    if (NextInstruction.InstructionArguments[0].Name.ToLower().Contains("pos")) NextInstruction.InstructionArguments[0].Enabled = true;
                    PlasmaProgram.Instructions.Add(NextInstruction);

                    for (int i = StartPlasma; i < PathTargets.Count - 1; i++)
                    {
                        RsMoveInstruction moveInstruction;
                        if (PathMoveTypes[i] == RsPointType.CirPoint || PathMoveTypes[i] == RsPointType.ViaPoint)
                        {
                            PathTargets[i].Transform.Z = VerticalOffset;
                            PathTargets[i + 1].Transform.Z = VerticalOffset;

                            moveInstruction = new RsMoveInstruction(station.ActiveTask, "Move", "Default", station.ActiveTask.ActiveWorkObject.Name, PathTargets[i].Name, PathTargets[i + 1].Name, station.ActiveTask.ActiveTool.Name);
                            i++;
                            if (moveInstruction.InstructionArguments.TryGetInstructionArgument("Speed", out InstructionToModify)) InstructionToModify.Value = cmbSpeed.SelectedItem.Text;
                            if (moveInstruction.InstructionArguments.TryGetInstructionArgument("Zone", out InstructionToModify)) InstructionToModify.Value = "z0";
                            PlasmaProgram.Instructions.Add(moveInstruction);
                        }
                        else
                        {
                            PathTargets[i].Transform.Z = VerticalOffset;
                            PlasmaProgram.Instructions.Add(CreateNewMove(station.ActiveTask, MotionType.Linear, PathTargets[i].Name, cmbSpeed.SelectedItem.Text, "z0"));
                        }
                        

                        
                    }

                    


                    NextInstruction = new RsActionInstruction(station.ActiveTask, "WaitTime", "Default");
                    if (NextInstruction.InstructionArguments.TryGetInstructionArgument("Time", out InstructionToModify)) InstructionToModify.Value = ".1";
                    if (NextInstruction.InstructionArguments[0].Name.ToLower().Contains("pos")) NextInstruction.InstructionArguments[0].Enabled = true;
                    PlasmaProgram.Instructions.Add(NextInstruction);


                    //PlasmaProgram.Instructions.Add();
                    NextInstruction = new RsActionInstruction(station.ActiveTask, "Reset", "Default");
                    if (NextInstruction.InstructionArguments.TryGetInstructionArgument("Signal", out InstructionToModify)) InstructionToModify.Value = cmbIOList.SelectedItem.Text;
                    PlasmaProgram.Instructions.Add(NextInstruction);


                    PlasmaProgram.Instructions.Add(CreateNewMove(station.ActiveTask,MotionType.Linear, PathTargets[PathTargets.Count - 1].Name,"v1000","z0"));

                }
            }

            NextInstruction = new RsActionInstruction(station.ActiveTask, "ConfL", "On");
            if (NextInstruction.InstructionArguments[0].Name.ToLower().Contains("pos")) NextInstruction.InstructionArguments[0].Enabled = true;
            PlasmaProgram.Instructions.Add(NextInstruction);

            NextInstruction = new RsActionInstruction(station.ActiveTask, "ConfJ", "On");
            if (NextInstruction.InstructionArguments[0].Name.ToLower().Contains("pos")) NextInstruction.InstructionArguments[0].Enabled = true;
            PlasmaProgram.Instructions.Add(NextInstruction);
        }


        static RsMoveInstruction CreateNewMove(RsTask RobotTask,MotionType MoveType,string Targetname,string Speed,string Zone)
        {
            RsInstructionArgument InstructionToModify;
            RsMoveInstruction NextMoveInstruction = new RsMoveInstruction(RobotTask, "Move", "Default",
                           MoveType, RobotTask.ActiveWorkObject.Name, Targetname, RobotTask.ActiveTool.Name);
            if (NextMoveInstruction.InstructionArguments.TryGetInstructionArgument("Speed", out InstructionToModify)) InstructionToModify.Value = Speed;
            if (NextMoveInstruction.InstructionArguments.TryGetInstructionArgument("Zone", out InstructionToModify)) InstructionToModify.Value = Zone;

            return NextMoveInstruction;

        }

        static RsMoveInstruction CreateNewCircleMove(RsTask RobotTask, MotionType MoveType, string Targetname1, string Targetname2, string Speed, string Zone)
        {
            RsInstructionArgument InstructionToModify;
            RsMoveInstruction NextMoveInstruction = new RsMoveInstruction(RobotTask, "Move", "Default", RobotTask.ActiveWorkObject.Name, Targetname1, Targetname2, RobotTask.ActiveTool.Name);
            if (NextMoveInstruction.InstructionArguments.TryGetInstructionArgument("Speed", out InstructionToModify)) InstructionToModify.Value = Speed;
            if (NextMoveInstruction.InstructionArguments.TryGetInstructionArgument("Zone", out InstructionToModify)) InstructionToModify.Value = Zone;

            return NextMoveInstruction;

        }


        static void CreateLayout()
        {
            Part DXFDrawing = new Part();
            //Part TempPart = new Part();
            List<Body> TempBodies = new List<Body>();
            List<Wire> DXFAssembly = new List<Wire>();
            List<Body> wireunion = new List<Body>();

            DXFDrawing.Name = "DXF Part";
            // Show and activate the add-in tab
            System.Windows.Forms.OpenFileDialog FileDialog1 = new System.Windows.Forms.OpenFileDialog();
            FileDialog1.Filter = "DXF Files (*.dxf)|*.dxf";
            FileDialog1.ShowDialog();
            if (FileDialog1.FileName == "") return;


            try
            {
                Station station = Station.ActiveStation;
                LoadDXF(ref DXFAssembly, FileDialog1.FileName);


                SortWires(ref DXFAssembly);

                wireunion.AddRange(Body.JoinCurves(DXFAssembly.ToArray(), .0001));

                foreach (Body item in wireunion)
                {
                    DXFDrawing.Bodies.Add(item);
                }
                DXFDrawing.Transform.GlobalMatrix = station.ActiveTask.ActiveWorkObject.ObjectFrame.GlobalMatrix;
                station.GraphicComponents.Add(DXFDrawing);
                Logger.AddMessage(new LogMessage("File imported successfully"));
            }
            catch (Exception E)
            {

                Logger.AddMessage(new LogMessage(E.Message.ToString()));
                //System.Windows.Forms.MessageBox.Show(E.Message.ToString());
            }



        }


        static void CreatePath()
        {
            double Kerf;
            Kerf = double.Parse(cmbKerf.SelectedItem.Text) / 1000;
            List<Body> TempBodies = new List<Body>();
            List<Wire> DXFAssembly = new List<Wire>();
            List<Body> wireunion = new List<Body>();



            System.Windows.Forms.OpenFileDialog FileDialog1 = new System.Windows.Forms.OpenFileDialog();
            FileDialog1.Filter = "DXF Files (*.dxf)|*.dxf";
            FileDialog1.ShowDialog();

            if (FileDialog1.FileName == "") return;

            try
            {
                Station station = Station.ActiveStation;
                Part DXFDrawing = new Part();

                DXFDrawing.Name = "DXF Part";

                LoadDXF(ref DXFAssembly, FileDialog1.FileName);


                SortWires(ref DXFAssembly);

                wireunion.AddRange(Body.JoinCurves(DXFAssembly.ToArray(), .0001));

                foreach (Body item in wireunion)
                {
                    Body ExtrudedBody = Body.Extrude(item.Wires[0], new Vector3(0, 0, -.001), null, new SweepOptions())[0];
                    ExtrudedBody.Transform.Z = .0005;
                    TempBodies.Add(ExtrudedBody);
                }

                Body FinishedBody;
                for (int i = 0; i < TempBodies.Count; i++)
                {

                    bool Success = true;
                    FinishedBody = TempBodies[i];
                    for (int x = 0; x < TempBodies.Count; x++)
                    {

                        if (x != i && TempBodies[x].BodyType == BodyType.SolidBody && TempBodies[i].BodyType == BodyType.SolidBody
                            && FinishedBody.Cut2(TempBodies[x]).Length > 0)
                            FinishedBody = FinishedBody.Cut2(TempBodies[x])[0];
                        else if (x != i && FinishedBody.Cut2(TempBodies[x]).Length == 0)
                            Success = false;
                    }
                    if (Success && FinishedBody.SurfaceArea > .001)
                    {
                        //FinishedBody.Defeature(.001, .001, .001, null);
                        DXFDrawing.Bodies.Add(Body.CreateIntersectionCurve(FinishedBody, Body.CreateSurfaceRectangle(new Matrix4(new Vector3(-5, -5, 0)), 10, 10)));
                    }
                }

                //foreach (Body item in wireunion)
                //{
                //    DXFDrawing.Bodies.Add(item);
                //}



                station.GraphicComponents.Add(DXFDrawing);
                RsPathProcedure PlasmaProgram = new RsPathProcedure(station.ActiveTask.GetValidRapidName("Plasma_Path", "_", 1));
                PlasmaProgram.ModuleName = "PlasmaMod";
                PlasmaProgram.Synchronize = true;

                station.ActiveTask.PathProcedures.Add(PlasmaProgram);
                PathGenerator(ref DXFDrawing, ref PlasmaProgram, Kerf, double.Parse(cmbOffset.SelectedItem.Text) / 1000);
                DXFDrawing.Transform.GlobalMatrix = station.ActiveTask.ActiveWorkObject.ObjectFrame.GlobalMatrix;
                station.GraphicComponents.Add(DXFDrawing);
                Logger.AddMessage(new LogMessage("File imported successfully"));
            }
            catch (Exception E)
            {

                Logger.AddMessage(new LogMessage(E.Message.ToString()));
                //System.Windows.Forms.MessageBox.Show(E.Message.ToString());
            }
        }
    }

    #endregion








}