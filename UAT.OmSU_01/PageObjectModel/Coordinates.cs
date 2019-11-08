using OpenQA.Selenium.Interactions.Internal;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITSK.Selenium.PageObjectModel
{
    public class Coordinates : ICoordinates
    {
        private ICoordinates _coordinates = null;
        public int OffsetX = 0;
        public int OffsetY = 0;

        public Coordinates(ICoordinates coordinates, int offsetX, int offsetY)
        {
            _coordinates = coordinates;
            this.OffsetX = offsetX;
            this.OffsetY = offsetY;
        }

        public object AuxiliaryLocator
        {
            get
            {
                return this._coordinates.AuxiliaryLocator;
            }
        }

        public Point LocationInDom
        {
            get
            {
                Point location = new Point(this._coordinates.LocationInDom.X, this._coordinates.LocationInDom.Y);
                location.Offset(this.OffsetX, this.OffsetY);
                return location;
            }
        }

        public Point LocationInViewport
        {
            get
            {
                Point location = new Point(this._coordinates.LocationInViewport.X, this._coordinates.LocationInViewport.Y);
                location.Offset(this.OffsetX, this.OffsetY);
                return location;
            }
        }

        public Point LocationOnScreen
        {
            get
            {
                Point location = new Point(this._coordinates.LocationOnScreen.X, this._coordinates.LocationOnScreen.Y);
                location.Offset(this.OffsetX, this.OffsetY);
                return location;
            }
        }
    }
}
