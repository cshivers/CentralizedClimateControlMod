﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace EnhancedTemperature
{
    public class GraphicsLoader
    {
        public static Graphic HotPipeAtlas = GraphicDatabase.Get<Graphic_Single>("Things/Building/Hot_AirPipe_Atlas", ShaderDatabase.Transparent);
        public static Graphic HotPipeOverlayAtlas = GraphicDatabase.Get<Graphic_Single>("Things/Building/Hot_AirPipe_Overlay_Atlas", ShaderDatabase.MetaOverlay);
        public static Graphic ColdPipeOverlayAtlas = GraphicDatabase.Get<Graphic_Single>("Things/Building/Cold_AirPipe_Overlay_Atlas", ShaderDatabase.MetaOverlay);
        public static Graphic FrozenPipeOverlayAtlas = GraphicDatabase.Get<Graphic_Single>("Things/Building/Frozen_AirPipe_Overlay_Atlas", ShaderDatabase.MetaOverlay);
        public static Graphic AnyPipeOverlayAtlas = GraphicDatabase.Get<Graphic_Single>("Things/Building/Any_AirPipe_Overlay_Atlas", ShaderDatabase.MetaOverlay);

        public static Graphic God = GraphicDatabase.Get<Graphic_Single>("God", ShaderDatabase.Transparent);

        public static GraphicPipe GraphicHotPipe = new GraphicPipe(GraphicsLoader.HotPipeAtlas, AirFlowType.Hot);
        public static GraphicPipe GraphicHotPipeClear = new GraphicPipe(GraphicsLoader.God, AirFlowType.Hot);

        public static GraphicPipe_Overlay GraphicHotPipeOverlay = new GraphicPipe_Overlay(HotPipeOverlayAtlas, AnyPipeOverlayAtlas, AirFlowType.Hot);
    }
}
