﻿#region SearchAThing.Sci, Copyright(C) 2016 Lorenzo Delana, License under MIT
/*
* The MIT License(MIT)
* Copyright(c) 2016 Lorenzo Delana, https://searchathing.com
*
* Permission is hereby granted, free of charge, to any person obtaining a
* copy of this software and associated documentation files (the "Software"),
* to deal in the Software without restriction, including without limitation
* the rights to use, copy, modify, merge, publish, distribute, sublicense,
* and/or sell copies of the Software, and to permit persons to whom the
* Software is furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
* FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
* DEALINGS IN THE SOFTWARE.
*/
#endregion

using MongoDB.Bson.Serialization.Attributes;
using SearchAThing.Sci;
using System;
using System.Runtime.Serialization;
using static System.Math;
using System.Linq;
using System.Collections.Generic;
using MongoDB.Bson;

namespace SearchAThing.Sci
{

    public class MeasureUnitWithDefaultTolerance
    {

        public ObjectId ObjectId { get { return ObjectId.Parse(Id); } }

        string _Id;
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id
        {
            get
            {
                if (_Id == null) _Id = ObjectId.GenerateNewId().ToString();
                return _Id;
            }
            set
            {
                _Id = value;
            }
        }

        public double DefaultTolerance { get; private set; }

        string _PQName;
        [BsonElement("PQ")]
        public string PQName
        {
            get
            {
                if (_PQName == null) _PQName = MU.PhysicalQuantity.ToString();
                return _PQName;
            }
            set
            {
                _PQName = value;
            }
        }

        [BsonElement("MU")]
        public string MUName
        {
            get { return MU.ToString(); }
            set
            {
                MU = PQCollection.PhysicalQuantities.First(w => w.Name == PQName).MeasureUnits.First(w => w.Name == value);
            }
        }

        [BsonIgnore]
        public MeasureUnit MU { get; private set; }

        public MeasureUnitWithDefaultTolerance(double _DefaultTolerance, MeasureUnit _MU)
        {
            MU = _MU;
            DefaultTolerance = _DefaultTolerance;
        }

        public MeasureUnitWithDefaultTolerance ConvertTo(MeasureUnit toMU)
        {
            if (MU.PhysicalQuantity.MUConversionType == MeasureUnitConversionTypeEnum.NonLinear)
                return new MeasureUnitWithDefaultTolerance(MU.PhysicalQuantity.NonLinearConversionFunctor(MU, toMU, DefaultTolerance), toMU);
            else
                return new MeasureUnitWithDefaultTolerance(DefaultTolerance * MU.PhysicalQuantity.ConvertFactor(MU, toMU), toMU);
        }

        public override string ToString()
        {
            return $"pq=[{PQName}] mu=[{MUName}] deftol=[{DefaultTolerance}]";
        }

    }

    public interface IMUDomain
    {

        IEnumerable<MeasureUnitWithDefaultTolerance> _All { get; }

        void SetupItem(string physicalQuantityName, string measureUnitName, double? defaultTolerance = null);

        MeasureUnitWithDefaultTolerance Length { get; set; }
        MeasureUnitWithDefaultTolerance Length2 { get; set; }
        MeasureUnitWithDefaultTolerance Mass { get; set; }
        MeasureUnitWithDefaultTolerance Time { get; set; }
        MeasureUnitWithDefaultTolerance ElectricCurrent { get; set; }
        MeasureUnitWithDefaultTolerance Temperature { get; set; }
        MeasureUnitWithDefaultTolerance AmountOfSubstance { get; set; }
        MeasureUnitWithDefaultTolerance LuminousIntensity { get; set; }

        //-------------------------------------------------------------------

        MeasureUnitWithDefaultTolerance PlaneAngle { get; set; }
        MeasureUnitWithDefaultTolerance Pressure { get; set; }
        MeasureUnitWithDefaultTolerance Acceleration { get; set; }
        MeasureUnitWithDefaultTolerance Force { get; set; }
        MeasureUnitWithDefaultTolerance Speed { get; set; }
        MeasureUnitWithDefaultTolerance Energy { get; set; }
        MeasureUnitWithDefaultTolerance Turbidity { get; set; }
        MeasureUnitWithDefaultTolerance Power { get; set; }
        MeasureUnitWithDefaultTolerance ElectricalConductance { get; set; }
        MeasureUnitWithDefaultTolerance ElectricalConductivity { get; set; }

    }

    /// <summary>
    /// Measures here contains information about implicit measure unit
    /// and value of the tolerance.
    /// 
    /// Note that all measure must be dimensionally equivalent.
    /// For example:
    /// [length] = m
    /// [length2] = [length] * [length] = m2
    /// [time] = s
    /// [time2] = [time] * [time] = s2
    /// [speed] = [length] / [time] = m/s
    /// [acceleration] = [length] / [time2] = m/s2
    /// [mass] = kg
    /// [force] = [mass] * [acceleration] = kg * m/s2 = N
    /// [pressure] = [force] / [length2] = N / m2 = Pa
    /// 
    /// This will ensure measure comparision without further conversion, for example
    /// m1 = 1 [kg]
    /// a1 = 2 [m/s2]
    /// f1 = 4 [N]
    /// 
    /// test = m1 * a1 > f1
    /// </summary>
    [DataContract(IsReference = true)]
    public class MUDomain : IMUDomain
    {

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public MeasureUnitWithDefaultTolerance Adimensional { get; set; }

        /// <summary>
        /// [L]
        /// </summary>
        [DataMember]
        public MeasureUnitWithDefaultTolerance Length { get; set; }

        /// <summary>
        /// [L^2]
        /// </summary>
        [DataMember]
        public MeasureUnitWithDefaultTolerance Length2 { get; set; }

        /// <summary>
        /// [M]
        /// </summary>
        [DataMember]
        public MeasureUnitWithDefaultTolerance Mass { get; set; }

        /// <summary>
        /// [T]
        /// </summary>
        [DataMember]
        public MeasureUnitWithDefaultTolerance Time { get; set; }

        /// <summary>
        /// [I]
        /// </summary>
        [DataMember]
        public MeasureUnitWithDefaultTolerance ElectricCurrent { get; set; }

        /// <summary>
        /// [K]
        /// </summary>
        [DataMember]
        public MeasureUnitWithDefaultTolerance Temperature { get; set; }

        /// <summary>
        /// [N]
        /// </summary>
        [DataMember]
        public MeasureUnitWithDefaultTolerance AmountOfSubstance { get; set; }

        /// <summary>
        /// [J]
        /// </summary>
        [DataMember]
        public MeasureUnitWithDefaultTolerance LuminousIntensity { get; set; }

        //------------------------------------------------------------------------------

        /// <summary>
        /// [1]
        /// </summary>
        [DataMember]
        public MeasureUnitWithDefaultTolerance PlaneAngle { get; set; }

        /// <summary>
        /// [M L−1 T−2]
        /// </summary>
        [DataMember]
        public MeasureUnitWithDefaultTolerance Pressure { get; set; }

        /// <summary>
        /// [L T−2]
        /// </summary>
        [DataMember]
        public MeasureUnitWithDefaultTolerance Acceleration { get; set; }

        /// <summary>
        /// [M L T−2]
        /// </summary>
        [DataMember]
        public MeasureUnitWithDefaultTolerance Force { get; set; }

        /// <summary>
        /// [L T−1]
        /// </summary>
        [DataMember]
        public MeasureUnitWithDefaultTolerance Speed { get; set; }

        /// <summary>
        /// [M L2 T−2]
        /// </summary>
        [DataMember]
        public MeasureUnitWithDefaultTolerance Energy { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public MeasureUnitWithDefaultTolerance Turbidity { get; set; }

        /// <summary>
        /// [M L2 T−3]
        /// </summary>
        [DataMember]
        public MeasureUnitWithDefaultTolerance Power { get; set; }

        /// <summary>
        /// [L−2 M−1 T3 I2]
        /// </summary>
        [DataMember]
        public MeasureUnitWithDefaultTolerance ElectricalConductance { get; set; }

        /// <summary>
        /// [L−3 M−1 T3 I2]
        /// </summary>
        [DataMember]
        public MeasureUnitWithDefaultTolerance ElectricalConductivity { get; set; }

        //------------------------------------------------------------------------------

        static Type tMeasureUnitWithDefaultTolerance = typeof(MeasureUnitWithDefaultTolerance);

        public IEnumerable<MeasureUnitWithDefaultTolerance> _All
        {
            get
            {
                var t = GetType();

                var props = t.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

                foreach (var prop in props.Where(r => r.PropertyType == tMeasureUnitWithDefaultTolerance))
                {
                    yield return (MeasureUnitWithDefaultTolerance)prop.GetMethod.Invoke(this, null);
                }
            }
        }

        /// <summary>
        /// allow to set programmatically the associated measure unit and tolerance in the model of a given physical quantity
        /// if given defaulttolreance is null, then current default tolerance will be converted to given measure unit
        /// </summary>        
        public void SetupItem(string physicalQuantityName, string measureUnitName, double? defaultTolerance = null)
        {
            var pq = PQCollection.PhysicalQuantities.First(w => w.Name == physicalQuantityName);

            var t = GetType();

            var props = t.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            var mu = MUCollection.MeasureUnits.First(w => w.Name == measureUnitName);

            MeasureUnitWithDefaultTolerance muwdt = null;

            if (defaultTolerance.HasValue)
            {
                muwdt = new MeasureUnitWithDefaultTolerance(defaultTolerance.Value, mu);
            }
            else
            {
                var curTol = props
                    .Where(r => r.PropertyType == tMeasureUnitWithDefaultTolerance)
                    .First(w => w.Name == physicalQuantityName)
                    .GetMethod.Invoke(this, null);

                muwdt = (curTol as MeasureUnitWithDefaultTolerance).ConvertTo(mu);
            }

            props
                .Where(r => r.PropertyType == tMeasureUnitWithDefaultTolerance)
                .First(w => w.Name == physicalQuantityName)
                .SetMethod.Invoke(this, new object[] { muwdt });
        }

        //------------------------------------------------------------------------------

        public MUDomain()
        {
            Adimensional = new MeasureUnitWithDefaultTolerance(0, MUCollection.Adimensional.adim);

            Length = new MeasureUnitWithDefaultTolerance(1e-4, MUCollection.Length.m);
            Mass = new MeasureUnitWithDefaultTolerance(1e-4, MUCollection.Mass.kg);
            Time = new MeasureUnitWithDefaultTolerance(1e-1, MUCollection.Time.sec);
            ElectricCurrent = new MeasureUnitWithDefaultTolerance(1e-9, MUCollection.ElectricCurrent.A);
            Temperature = new MeasureUnitWithDefaultTolerance(1e-1, MUCollection.Temperature.C);
            AmountOfSubstance = new MeasureUnitWithDefaultTolerance(1e-9, MUCollection.AmountOfSubstance.mol);
            LuminousIntensity = new MeasureUnitWithDefaultTolerance(1e-9, MUCollection.LuminousIntensity.cd);

            //---------------------------------------------------------------

            Length2 = new MeasureUnitWithDefaultTolerance(1e-4, MUCollection.Length2.m2);
            Force = new MeasureUnitWithDefaultTolerance(1e-1, MUCollection.Force.N);
            PlaneAngle = new MeasureUnitWithDefaultTolerance(PI / 180.0 / 10.0, MUCollection.PlaneAngle.rad);
            Pressure = new MeasureUnitWithDefaultTolerance(1e-1, MUCollection.Pressure.Pa);
            Acceleration = new MeasureUnitWithDefaultTolerance(1e-1, MUCollection.Acceleration.m_s2);
            Speed = new MeasureUnitWithDefaultTolerance(1e-1, MUCollection.Speed.m_s);
            Energy = new MeasureUnitWithDefaultTolerance(1e-4, MUCollection.Energy.J);
            Power = new MeasureUnitWithDefaultTolerance(1e-4, MUCollection.Power.W);
            ElectricalConductance = new MeasureUnitWithDefaultTolerance(1e-9, MUCollection.ElectricalConductance.S);
            ElectricalConductivity = new MeasureUnitWithDefaultTolerance(1e-9, MUCollection.ElectricalConductivity.S_m);
            Turbidity = new MeasureUnitWithDefaultTolerance(1e-9, MUCollection.Turbidity.FNU);
        }

    }

    public static partial class Extensions
    {

        public static MeasureUnitWithDefaultTolerance ByPhysicalQuantity(this IMUDomain mud, PhysicalQuantity physicalQuantity)
        {
            var id = physicalQuantity.id;

            if (mud.Length.MU.PhysicalQuantity.id == id) return mud.Length;
            else if (mud.Length2.MU.PhysicalQuantity.id == id) return mud.Length2;
            else if (mud.Mass.MU.PhysicalQuantity.id == id) return mud.Mass;
            else if (mud.Time.MU.PhysicalQuantity.id == id) return mud.Time;
            else if (mud.ElectricCurrent.MU.PhysicalQuantity.id == id) return mud.ElectricCurrent;
            else if (mud.Temperature.MU.PhysicalQuantity.id == id) return mud.Temperature;
            else if (mud.AmountOfSubstance.MU.PhysicalQuantity.id == id) return mud.AmountOfSubstance;
            else if (mud.LuminousIntensity.MU.PhysicalQuantity.id == id) return mud.LuminousIntensity;

            //---------------------------------------------------------------

            else if (mud.PlaneAngle.MU.PhysicalQuantity.id == id) return mud.PlaneAngle;
            else if (mud.Pressure.MU.PhysicalQuantity.id == id) return mud.Pressure;
            else if (mud.Acceleration.MU.PhysicalQuantity.id == id) return mud.Acceleration;
            else if (mud.Force.MU.PhysicalQuantity.id == id) return mud.Force;
            else if (mud.Speed.MU.PhysicalQuantity.id == id) return mud.Speed;
            else if (mud.Energy.MU.PhysicalQuantity.id == id) return mud.Energy;
            else if (mud.Power.MU.PhysicalQuantity.id == id) return mud.Power;
            else if (mud.ElectricalConductance.MU.PhysicalQuantity.id == id) return mud.ElectricalConductance;
            else if (mud.ElectricalConductivity.MU.PhysicalQuantity.id == id) return mud.ElectricalConductivity;
            else if (mud.Turbidity.MU.PhysicalQuantity.id == id) return mud.Turbidity;

            throw new NotImplementedException($"unable to find measure domain for given physical quantity {physicalQuantity}");
        }

    }

}
