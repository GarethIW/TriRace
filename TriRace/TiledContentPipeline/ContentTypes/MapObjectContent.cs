﻿using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;
using System;

namespace TiledContentPipeline
{
    public enum ObjectType
    {
        Rectangle,
        PolyLine,
        Polygon
    }

	public class MapObjectContent
	{
		public string Name = string.Empty;
		public string Type = string.Empty;
        public ObjectType ObjectType;
		public Rectangle Location;
        public List<Point> LinePoints = new List<Point>();
        public List<Point> PolyPoints = new List<Point>();
		public List<Property> Properties = new List<Property>();

		public MapObjectContent(XmlNode node)
		{
            if (node.Attributes["name"] != null)
            {
                Name = node.Attributes["name"].Value;
            }

            if (node.Attributes["type"] != null)
            {
                Type = node.Attributes["type"].Value;
            }

            Location = new Rectangle(
                    int.Parse(node.Attributes["x"].Value),
                    int.Parse(node.Attributes["y"].Value),
                    0,
                    0);

            XmlNode polyLineNode = node["polyline"];
            XmlNode polygonNode = node["polygon"];
            if (polyLineNode != null)
            {
                // Polyline node
                ObjectType = ObjectType.PolyLine;

                

                string pointsString = polyLineNode.Attributes["points"].Value;
                foreach (string point in pointsString.Split(' '))
                {
                    LinePoints.Add(new Point(Location.X + Convert.ToInt32(point.Split(',')[0]), Location.Y + Convert.ToInt32(point.Split(',')[1])));
                }
            }
            else if (polygonNode != null)
            {
                // Polygon node
                ObjectType = ObjectType.Polygon;

                string pointsString = polygonNode.Attributes["points"].Value;
                foreach (string point in pointsString.Split(' '))
                {
                    PolyPoints.Add(new Point(Location.X + Convert.ToInt32(point.Split(',')[0]), Location.Y + Convert.ToInt32(point.Split(',')[1])));
                }
            }
            else
            {
                ObjectType = ObjectType.Rectangle;
                
                Location = new Rectangle(
                    int.Parse(node.Attributes["x"].Value),
                    int.Parse(node.Attributes["y"].Value),
                    int.Parse(node.Attributes["width"].Value),
                    int.Parse(node.Attributes["height"].Value));
            }

			XmlNode propertiesNode = node["properties"];
			if (propertiesNode != null)
			{
				foreach (XmlNode property in propertiesNode.ChildNodes)
				{
					Properties.Add(new Property
					{
						Name = property.Attributes["name"].Value,
						Value = property.Attributes["value"].Value,
					});
				}
			}
		}
	}
}
