using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using CinchExtended.BusinessObjects.Bases;
using CinchExtended.Utilities;
using CinchExtended.ViewModels;

namespace CinchExtended.BusinessObjects
{

    /// <summary>
    /// Abstract base class for DataWrapper which will support IsDirty. So in your ViewModel
    /// you could do something like
    /// 
    /// <example>
    /// <![CDATA[
    /// 
    ///public bool IsDirty
    ///{
    ///   get
    ///   {
    ///     return cachedListOfDataWrappers.Where(x => (x is IChangeIndicator)
    ///     &&  ((IChangeIndicator)x).IsDirty).Count() > 0;
    ///   }
    ///
    /// } 
    /// ]]>
    /// </example>
    /// </summary>
    public abstract class DataWrapperDirtySupportingBase : EditableValidatingObject
    {
        #region Public Properties
        /// <summary>
        /// Deteremines if a property has changes since is was put into edit mode
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <returns>True if the property has changes since is was put into edit mode</returns>
        /// <summary>
        /// Deteremines if a property has changes since is was put into edit mode
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <returns>True if the property has changes since is was put into edit mode</returns>
        public bool HasPropertyChanged(string propertyName)
        {
            string _propertyName = propertyName.ToLower();

            if (_savedState == null)
                return false;

            object saveValue;
            object currentValue;

            if (!_savedState.TryGetValue(_propertyName, out saveValue) ||
            !this.GetFieldValues().TryGetValue(_propertyName, out currentValue))
                return false;

            if (saveValue == null || currentValue == null)
            {
                if (saveValue is string || currentValue is string)
                {
                    return (string)(saveValue ?? string.Empty) != (string)(currentValue ?? string.Empty);
                }
                else
                {
                    return saveValue != currentValue;
                }
            }

            return !saveValue.Equals(currentValue);


        }
        #endregion



        protected override Dictionary<string, object> GetFieldValues()
        {
            Dictionary<string, object> _propertyValues = null;

            // Get all Properties. The Instance flag is what causes everything to return.
            var _props = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            if (_props.Count() > 0)
            {
                _propertyValues = new Dictionary<string, object>();

                // Only get a DataWrapper that has a datavalue property
                var _property = _props.SingleOrDefault(p => p.Name.ToLower() == "datavalue");

                // Only add Primitives, Strings & objects right now.
                // Lists or collection don't get added. You'll have to call BeginEdit() on each object you add to a list when it's
                // contained in a parent object.
                // TODO here figure out how if we get a DataWrapper which is a collection, how to loop through that collection of objects and
                // set them to edit mode as well.
                if (_property != null && !_property.PropertyType.IsSubclassOf(typeof(IEnumerable)) && 
                    (_property.PropertyType.BaseType == typeof(ValueType) || _property.PropertyType.IsSubclassOf(typeof(object))))
                {
                    _propertyValues.Add(_property.Name.ToLower(), _property.GetValue(this, null));
                }
                else
                {
                    Debug.WriteLine(string.Format("Property did not contain a DataValue property. List of properties{0}", _props.ToString()));
                }
            }

            return _propertyValues;
        }


        protected override void RestoreFieldValues(Dictionary<string, object> fieldValues)
        {
            foreach (PropertyInfo pi in GetType().GetProperties(
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(pi => pi.CanWrite && pi.GetIndexParameters().Length == 0))
            {
                object value;
                if (fieldValues.TryGetValue(pi.Name.ToLower(), out value))
                    pi.SetValue(this, value, null);
                else
                {
                    Debug.WriteLine("Failed to restore property " +
                    pi.Name + " from cloned values, property not found in Dictionary.");
                }
            }

        }

    }



    /// <summary>
    /// Abstract base class for DataWrapper - allows easier access to
    /// methods for the DataWrapperHelper.
    /// </summary>
    public abstract class DataWrapperBase : DataWrapperDirtySupportingBase
    {
        #region Data
        private Boolean isEditable = true;
        private Boolean isReadOnly = false;
        private IParentablePropertyExposer parent = null;
        private readonly PropertyChangedEventArgs parentPropertyChangeArgs = null;
        private Action valuesChangedCallBack = null;
        #endregion

        #region Ctors
        public DataWrapperBase(IParentablePropertyExposer parent,
            PropertyChangedEventArgs parentPropertyChangeArgs)
        {
            this.parent = parent;
            this.parentPropertyChangeArgs = parentPropertyChangeArgs;
        }

        /// <summary>
        /// This constructor is intended for use with Partial classes
        /// as generated by the Cinch code generator
        /// </summary>
        public DataWrapperBase(IParentablePropertyExposer parent,
            PropertyChangedEventArgs parentPropertyChangeArgs, Action valuesChangedCallBack)
        {
            this.parent = parent;
            this.parentPropertyChangeArgs = parentPropertyChangeArgs;
            this.valuesChangedCallBack = valuesChangedCallBack;
        }
        #endregion

        #region Protected Methods

        /// <summary>
        /// Notifies all the parent (INPC) objects INotifyPropertyChanged.PropertyChanged subscribed delegates
        /// that an internal DataWrapper property value has changed, which in turn raises the appropriate
        /// INotifyPropertyChanged.PropertyChanged event on the parent (INPC) object
        /// </summary>
        protected internal void NotifyParentPropertyChanged()
        {
            if (parent == null || ParentPropertyChangeArgs == null)
                return;

            //notify all delegates listening to DataWrapper<T> parent objects PropertyChanged
            //event
            Delegate[] subscribers = parent.GetINPCSubscribers();
            if (subscribers != null)
            {
                foreach (PropertyChangedEventHandler d in subscribers)
                {
                    d(parent, ParentPropertyChangeArgs);
                }
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// This indicated whether the DataWrapper is Readonly. This is expected
        /// to be the inverse of the IsEditable property. This property has been
        /// added for convenience, so you do not have to use a ValueConverter in
        /// your XAML to convert IsEditable -> ReadOnly in controls that support
        /// ReadOnly such as textBox
        /// </summary>
        static PropertyChangedEventArgs isReadOnlyChangeArgs =
            ObservableHelper.CreateArgs<DataWrapperBase>(x => x.IsReadOnly);

        public Boolean IsReadOnly
        {
            get { return isReadOnly; }
            set
            {
                if (isReadOnly != value)
                {
                    isReadOnly = value;
                    NotifyPropertyChanged(isReadOnlyChangeArgs);
                    NotifyParentPropertyChanged();
                    if (valuesChangedCallBack != null)
                        valuesChangedCallBack();
                }
            }

        }



        /// <summary>
        /// The editable state of the data, the View
        /// is expected to use this to enable/disable
        /// data entry. The ViewModel would set this
        /// property
        /// </summary>
        static PropertyChangedEventArgs isEditableChangeArgs =
            ObservableHelper.CreateArgs<DataWrapperBase>(x => x.IsEditable);

        public Boolean IsEditable
        {
            get { return isEditable; }
            set
            {
                if (isEditable != value)
                {
                    isEditable = value;
                    NotifyPropertyChanged(isEditableChangeArgs);
                    NotifyParentPropertyChanged();
                    if (valuesChangedCallBack != null)
                        valuesChangedCallBack();

                    //now set IsReadOnly too
                    IsReadOnly = !IsEditable;
                }
            }

        }

        public PropertyChangedEventArgs ParentPropertyChangeArgs
        {
            get { return parentPropertyChangeArgs; }
        }

        #endregion
    }

    /// <summary>
    /// This interface is here so to ensure that both DataWrapper of T
    /// and DataWrapperExt of T have a commonly named property for
    /// the data (DataValue) and that we can safely retrieve this
    /// name elsewhere via static reflection.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDataWrapper<T>
    {
        T DataValue { get; set; }
    }

    /// <summary>
    /// Allows IsDierty to be determined for a cached list of DataWrappers
    /// </summary>
    public interface IChangeIndicator
    {
        bool IsDirty { get; }
    }


    /// <summary>
    /// This interface is implemented by both the 
    /// <see cref="ValidatingObject">ValidatingObject</see> and the
    /// <see cref="ViewModelBase">ViewModelBase</see> classes, and is used
    /// to expose the list of delegates that are currently listening to the
    /// <see cref="System.ComponentModel.INotifyPropertyChanged">INotifyPropertyChanged</see>
    /// PropertyChanged event. This is done so that the internal 
    /// <see cref="DataWrapper">DataWrapper</see> classes can notify their parent object
    /// when an internal <see cref="DataWrapper">DataWrapper</see> property changes
    /// </summary>
    public interface IParentablePropertyExposer
    {
        Delegate[] GetINPCSubscribers();
    }


    /// <summary>
    /// Provides a wrapper around a single piece of data
    /// such that the ViewModel can put the data item
    /// into a editable state and the View can bind to
    /// both the DataValue for the actual Value, and to 
    /// the IsEditable to determine if the control which
    /// has the data is allowed to be used for entering data.
    /// 
    /// The Viewmodel is expected to set the state of the
    /// IsEditable property for all DataWrappers in a given Model
    /// </summary>
    /// <typeparam name="T">The type of the Data</typeparam>
    public partial class DataWrapper<T> : DataWrapperBase, IDataWrapper<T>, IChangeIndicator
    {
        #region Data
        private T dataValue = default(T);
        private Action valuesChangedCallBack = null;
        private object parentViewModel = null;
        #endregion

        #region Ctors
        public DataWrapper(IParentablePropertyExposer parent,
            PropertyChangedEventArgs parentPropertyChangeArgs)
            : base(parent, parentPropertyChangeArgs)
        {
            parentViewModel = (object)parent;
        }

        /// <summary>
        /// This constructor is intended for use with Partial classes
        /// as generated by the Cinch code generator
        /// </summary>
        public DataWrapper(IParentablePropertyExposer parent,
            PropertyChangedEventArgs parentPropertyChangeArgs, Action valuesChangedCallBack)
            : base(parent, parentPropertyChangeArgs, valuesChangedCallBack)
        {
            this.valuesChangedCallBack = valuesChangedCallBack;
            parentViewModel = (object)parent;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// The actual data value, the View is
        /// expected to bind to this to display data
        /// </summary>
        static PropertyChangedEventArgs dataValueChangeArgs =
            ObservableHelper.CreateArgs<DataWrapper<T>>(x => x.DataValue);

        public T DataValue
        {
            get { return dataValue; }
            set
            {
                dataValue = value;
                NotifyPropertyChanged(dataValueChangeArgs);
                NotifyParentPropertyChanged();
                if (valuesChangedCallBack != null)
                    valuesChangedCallBack();
            }
        }

        public bool IsDirty
        {
            get { return this.HasPropertyChanged("dataValue"); }
        }

        public object ParentViewModel
        {
            get { return parentViewModel; }
        }
        #endregion
    }


    /// <summary>
    /// Provides helper methods for dealing with DataWrappers
    /// within the Cinch library. 
    /// </summary>
    public class DataWrapperHelper
    {
        #region Public Methods
        // The following functions may be used when dealing with model/viewmodel objects
        // whose entire set of DataWrapper properties are immutable (only have a getter
        // for the property).  They avoid having to do reflection to retrieve the list
        // of wrapper properties every time a mode change, edit state change

        /// <summary>
        /// Set all Cinch.DataWrapper properties to have the correct Cinch.DataWrapper.IsEditable 
        /// to the correct state based on the current ViewMode 
        /// </summary>
        /// <param name="wrapperProperties">The properties on which to change the mode</param>
        /// <param name="currentViewMode">The current ViewMode</param
        public static void SetMode(IEnumerable<DataWrapperBase> wrapperProperties,
            ViewMode currentViewMode)
        {
            bool isEditable = currentViewMode ==
                    ViewMode.EditMode || currentViewMode == ViewMode.AddMode;

            foreach (var wrapperProperty in wrapperProperties)
            {
                try
                {
                    wrapperProperty.IsEditable = isEditable;
                }
                catch (Exception)
                {
                    Debug.WriteLine("There was a problem setting the currentViewMode");
                }
            }
        }

        /// <summary>
        /// Loops through a source object (UI Model class is expected really) and attempts
        /// to call the BeginEdit() method of all the  Cinch.DataWrapper fields
        /// </summary>
        /// <param name="wrapperProperties">The DataWrapperBase objects</param>
        public static void SetBeginEdit(IEnumerable<DataWrapperBase> wrapperProperties)
        {
            foreach (var wrapperProperty in wrapperProperties)
            {
                try
                {
                    wrapperProperty.BeginEdit();
                    wrapperProperty.NotifyParentPropertyChanged();
                }
                catch (Exception)
                {
                    Debug.WriteLine("There was a problem calling the BeginEdit method for the current DataWrapper");
                }
            }
        }

        /// <summary>
        /// Loops through a source object (UI Model class is expected really) and attempts
        /// to call the CancelEdit() method of all the  Cinch.DataWrapper fields
        /// </summary>
        /// <param name="wrapperProperties">The DataWrapperBase objects</param>
        public static void SetCancelEdit(IEnumerable<DataWrapperBase> wrapperProperties)
        {
            foreach (var wrapperProperty in wrapperProperties)
            {
                try
                {
                    wrapperProperty.CancelEdit();
                    wrapperProperty.NotifyParentPropertyChanged();
                }
                catch (Exception)
                {
                    Debug.WriteLine("There was a problem calling the CancelEdit method for the current DataWrapper");
                }
            }
        }

        /// <summary>
        /// Loops through a source object (UI Model class is expected really) and attempts
        /// to call the EditEdit() method of all the  Cinch.DataWrapper fields
        /// </summary>
        /// <param name="wrapperProperties">The DataWrapperBase objects</param>
        public static void SetEndEdit(IEnumerable<DataWrapperBase> wrapperProperties)
        {
            foreach (var wrapperProperty in wrapperProperties)
            {
                try
                {
                    wrapperProperty.EndEdit();
                    wrapperProperty.NotifyParentPropertyChanged();
                }
                catch (Exception)
                {
                    Debug.WriteLine("There was a problem calling the EndEdit method for the current DataWrapper");
                }
            }
        }


        /// <summary>
        /// Loops through a source object (UI Model class is expected really) and attempts
        /// to call the EditEdit() method of all the  Cinch.DataWrapper fields
        /// </summary>
        /// <param name="wrapperProperties">The DataWrapperBase objects</param>
        public static Boolean AllValid(IEnumerable<DataWrapperBase> wrapperProperties)
        {

            Boolean allValid = true;

            foreach (var wrapperProperty in wrapperProperties)
            {
                try
                {
                    allValid &= wrapperProperty.IsValid;
                    if (!allValid)
                        break;
                }
                catch (Exception)
                {
                    allValid = false;
                    Debug.WriteLine("There was a problem calling the IsValid method for the current DataWrapper");
                }
            }

            return allValid;
        }



        /// <summary>
        /// Get a list of the wrapper properties on the parent object.
        /// </summary>
        /// <typeparam name="T">The type of object</typeparam>
        /// <param name="parentObject">The parent object to examine</param>
        /// <returns>A IEnumerable of DataWrapperBase</returns>
        public static IEnumerable<DataWrapperBase> GetWrapperProperties<T>(T parentObject)
        {
            var properties = parentObject.GetType().GetProperties(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            List<DataWrapperBase> wrapperProperties = new List<DataWrapperBase>();

            foreach (var propItem in parentObject.GetType().GetProperties(
                           BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                // check make sure can read and that the property is not an indexed property
                if (propItem.CanRead && propItem.GetIndexParameters().Count() == 0)
                {
                    // we ignore any property whose type CANNOT store a DataWrapper;
                    // this means any property whose type is not in the inheritance hierarchy
                    // of DataWrapper.  For example a property of type Object could potentially
                    // store a DataWrapper since Object is in DataWrapper's inheritance tree.
                    // However, a boolean property CANNOT since it's not in the wrapper's
                    // inheritance tree.
                    if (typeof(DataWrapperBase).IsAssignableFrom(propItem.PropertyType) == false)
                        continue;

                    // make sure properties value is not null ref
                    var propertyValue = propItem.GetValue(parentObject, null);
                    if (propertyValue != null && propertyValue is DataWrapperBase)
                    {
                        wrapperProperties.Add((DataWrapperBase)propertyValue);
                    }
                }
            }

            return wrapperProperties;
        }
        #endregion
    }
}