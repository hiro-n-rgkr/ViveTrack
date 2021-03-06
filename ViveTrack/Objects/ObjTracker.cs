﻿using System;
using System.Collections.Generic;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Rhino.Geometry;
using ViveTrack.Properties;

namespace ViveTrack.Objects
{
    public class ObjTracker : GH_Component
    {
        public GeometryBase tracker;
        public VrTrackedDevice CurrenTrackedDevice;
        public Plane XyPlane;
        public bool Paused = false;
        private Plane OldPlane;
        private Transform OldTransform;
        /// <summary>
        /// Initializes a new instance of the ObjTracker class.
        /// </summary>
        public ObjTracker()
          : base("Tracker", "Tracker",
              "HTC Vive Tracker 3D model",
              "ViveTrack", "Tracking Device")
        {
            tracker = GH_Convert.ByteArrayToCommonObject<GeometryBase>(System.Convert.FromBase64String(Resources.tracker));
            CurrenTrackedDevice = new VrTrackedDevice();
            //(this as IGH_PreviewObject).Hidden = true;
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Vive", "Vive", "Passing Vive Runtime to current component", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Index", "Index","If you have more than one tracker, please indicate the index of trackers. 0,1,2...",GH_ParamAccess.item, 0);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGeometryParameter("Tracker", "Tracker", "HTC Vive Tracker 3D model", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Plane", "Plane", "The Tracker plane representation", GH_ParamAccess.item);
            pManager.AddGenericParameter("Matrix", "Matrix", "Transformation matrix of Tracker", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            OpenvrWrapper temp = null;
            int index = 0;
            if (!DA.GetData("Vive", ref temp)) return;
            DA.GetData("Index", ref index);
            var list = temp.TrackedDevices.IndexesByClasses["Tracker"];
            if (list.Count == 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No Tracker deteceted");
                this.Message = "";
                return;
            }
            if (index > list.Count - 1)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Index exceeds the trackers detected");
                this.Message = "";
                return;
            }

            int globaleindex = temp.TrackedDevices.IndexesByClasses["Tracker"][index];
            CurrenTrackedDevice = temp.TrackedDevices.AllDevices[globaleindex];
            this.Message = "Tracker" + index;
            CurrenTrackedDevice.ConvertPose();
            CurrenTrackedDevice.GetTrackerCorrectedMatrix4X4();
            XyPlane = Plane.WorldXY;
            XyPlane.Transform(CurrenTrackedDevice.CorrectedMatrix4X4);
            if (!Paused)
            {
                OldPlane = XyPlane;
                OldTransform = CurrenTrackedDevice.CorrectedMatrix4X4;
            }

            var newtracker = tracker.Duplicate();
            newtracker.Transform(OldTransform);
            DA.SetData("Tracker", newtracker);
            DA.SetData("Plane", OldPlane);
            DA.SetData("Matrix", OldTransform);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return Resources.ObjTracker;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{bbe2bc88-530b-4f06-bca9-d77ca9db52da}"); }
        }

        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);
            Menu_AppendItem(menu, "Pause", Menu_Click_Pause, true, false).ToolTipText = @"Click Pause to pause the updation of current tracking device.";


        }

        private void Menu_Click_Pause(object sender, EventArgs e)
        {
            this.RecordUndoEvent("Pause");
            Paused = !Paused;
            this.ExpireSolution(true);
        }

        public override bool Write(GH_IWriter writer)
        {
            writer.SetBoolean("Pause", Paused);
            return base.Write(writer);
        }
    }
}