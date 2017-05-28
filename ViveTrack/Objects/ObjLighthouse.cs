﻿using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using ViveTrack.Properties;

namespace ViveTrack.Objects
{
    public class ObjLighthouse : GH_Component
    {
        public GeometryBase lighthouse;
        /// <summary>
        /// Initializes a new instance of the ObjLighthouse class.
        /// </summary>
        public ObjLighthouse()
          : base("ObjLighthouse", "ObjLighthouse",
              "Description",
              "ViveTrack", "Objects")
        {
            lighthouse = GH_Convert.ByteArrayToCommonObject<GeometryBase>(System.Convert.FromBase64String(Resources.lighthouse));
            this.Hidden = Hidden;
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGeometryParameter("Lighthouse", "Lighthouse", "", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            DA.SetData(0, lighthouse);
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
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{bfe9589b-d69d-43b2-9754-5eb6a08af0b3}"); }
        }
    }
}