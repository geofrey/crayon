﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crayon.ParseTree;
using Crayon.Translator;

namespace Crayon.Translator.Java
{
	internal class JavaAndroidOpenGlEsTranslator : AbstractOpenGlTranslator
	{
		public override void TranslateGlBeginPolygon(List<string> output)
		{
			output.Add("GlUtil.beginPolygon()");
		}

		public override void TranslateGlBeginQuads(List<string> output)
		{
			output.Add("GlUtil.beginQuads()");
		}

		public override void TranslateGlBindTexture(List<string> output, Expression textureId)
		{
			output.Add("GlUtil.glBindTexture(");
			this.Translator.TranslateExpression(output, textureId);
			output.Add(")");
		}

		public override void TranslateGlColor4(List<string> output, Expression r, Expression g, Expression b, Expression a)
		{
			output.Add("GlUtil.color4(");
			this.Translator.TranslateExpression(output, r);
			output.Add(", ");
			this.Translator.TranslateExpression(output, g);
			output.Add(", ");
			this.Translator.TranslateExpression(output, b);
			output.Add(", ");
			this.Translator.TranslateExpression(output, a);
			output.Add(")");
		}

		public override void TranslateGlDisableTexture2D(List<string> output)
		{
			output.Add("GlUtil.glDisableTexture2D()");
		}

		public override void TranslateGlEnableTexture2D(List<string> output)
		{
			output.Add("GlUtil.glEnableTexture2D()");
		}

		public override void TranslateGlEnd(List<string> output)
		{
			output.Add("GlUtil.glEnd()");
		}

		public override void TranslateGlLoadTexture(List<string> output, Expression platformBitmapResource)
		{
			output.Add("GlUtil.forceLoadTexture((android.graphics.Bitmap) ");
			this.Translator.TranslateExpression(output, platformBitmapResource);
			output.Add(")");
		}

		public override void TranslateGlMaxTextureSize(List<string> output)
		{
			output.Add("GlUtil.getMaxTextureSize()");
		}

		public override void TranslateGlTexCoord2(List<string> output, Expression x, Expression y)
		{
			output.Add("GlUtil.glTexCoord2(");
			this.Translator.TranslateExpression(output, x);
			output.Add(", ");
			this.Translator.TranslateExpression(output, y);
			output.Add(")");
		}

		public override void TranslateGlVertex2(List<string> output, Expression x, Expression y)
		{
			output.Add("GlUtil.glVertex2(");
			this.Translator.TranslateExpression(output, x);
			output.Add(", ");
			this.Translator.TranslateExpression(output, y);
			output.Add(")");
		}
	}
}