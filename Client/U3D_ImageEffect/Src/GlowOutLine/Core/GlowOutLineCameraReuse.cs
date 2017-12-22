﻿using System;
using UnityEngine;
namespace ImageEffects
{
    public class GlowOutLineCameraReuse : GlowOutLineCamera
    {
        public RenderBuffer depthBuffer;
        public RenderTexture screenRt;
        public RenderTexture tmpRt;
        public RenderTexture destion;
        public RenderTexture excratRt;
        public Camera HostCamera = null;

        internal override void Init()
        {
            HostCamera = GetComponent<Camera>();
        }

        public void OnPostRender()
        {
            RenderTexture temp2 = RenderTexture.GetTemporary(screenRt.width / 2, screenRt.height / 2);

            base.blur.BlurAndBlitBuffer(screenRt, tmpRt, temp2, destion, base.settings, base.highPrecision);
            RenderTexture.ReleaseTemporary(tmpRt);
            RenderTexture.ReleaseTemporary(temp2);
        }

        public void OnPreCull()
        {
            HostCamera.CopyFrom(base.parentCamera);
            HostCamera.backgroundColor = Color.black;
            HostCamera.clearFlags = CameraClearFlags.Nothing;
            HostCamera.SetReplacementShader(base.glowOutLineOnly, "GlowOutLine");
            HostCamera.renderingPath = RenderingPath.VertexLit;
            if (glowOutLineManager.DebugMode)
            {
                HostCamera.cullingMask = -1;
            }
            else
            {
                HostCamera.cullingMask = (1 << LayerMask.NameToLayer("LightingEffectOnly"));
            }
           

            tmpRt = RenderTexture.GetTemporary(screenRt.width, screenRt.height);
            RenderTexture.active = tmpRt;
            GL.Clear(false, true, Color.black);
            HostCamera.targetTexture = tmpRt;
            HostCamera.SetTargetBuffers(tmpRt.colorBuffer, depthBuffer);
        }

        public void OnPreRender()
        {
            Camera HostCamera = GetComponent<Camera>();
            HostCamera.projectionMatrix = base.parentCamera.projectionMatrix;
        }
    }
}

//using System;
//using UnityEngine;
//namespace GlowEffect
//{
//    public class GlowCameraReuse : BaseGlowCamera
//    {
//        private GlowCameraReuseHelper helper;

//        [HideInInspector]
//        public RenderTexture screenRt;
//        private RenderTexture tmpRt;

//        public  void ActivateHelper()
//        {
//            if ((parentCamera != null) && (helper == null))
//            {
//                helper = ImageEffectManager.AddImageEffect<GlowCameraReuseHelper>(parentCamera,true);
//                if (null != helper)
//                {
//                    helper.isFullScreenEffect = true;
//                    helper.hideFlags = HideFlags.HideInInspector;
//                    helper.glowCam = this;
//                }
//            }
//        }

//        internal override void Init()
//        {
//            ActivateHelper();
//        }

//        public void OnDeActive()
//        {
//            if (helper)
//            {
//                helper.glowCam = null;
//                ImageEffectManager.DeleteImageEffect(parentCamera, helper);
//            }
//        }

//        public void OnActive()
//        {
//            ActivateHelper();
//        }

//        public override void ImageEffect_PostRender()
//        {
//            blur.BlurAndBlitBuffer(tmpRt, screenRt, settings, highPrecision);
//            RenderTexture.ReleaseTemporary(tmpRt);
//        }

//        public override void ImageEffect_PreCull()
//        {
//            ActivateHelper();
//            if (screenRt == null)
//            {
//                return;
//            }
            
//            glowManager.glowCam.CopyFrom(parentCamera);
//            glowManager.glowCam.backgroundColor = Color.black;
//            glowManager.glowCam.clearFlags = CameraClearFlags.Nothing;
//            glowManager.glowCam.SetReplacementShader(glowOnly, "RenderEffect");
//            glowManager.glowCam.renderingPath = RenderingPath.VertexLit;

//            tmpRt = RenderTexture.GetTemporary(screenRt.width, screenRt.height);
//            RenderTexture.active = tmpRt;
//            GL.Clear(false, true, Color.black);
//            glowManager.glowCam.targetTexture = tmpRt;
//            glowManager.glowCam.SetTargetBuffers(tmpRt.colorBuffer, screenRt.depthBuffer);
//        }

//        public override void ImageEffect_PreRender()
//        {
//            glowManager.glowCam.projectionMatrix = parentCamera.projectionMatrix;
//        }
//    }
//}
