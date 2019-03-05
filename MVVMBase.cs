using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace JustMVVM
{
    [Serializable]
    public abstract class MVVMBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Event the fires when notifying UI the property has changed
        /// </summary>
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Calls the event handler to show that a property has changed
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Calls the event handler to show that a property has changed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyExpresssion"></param>
        protected void OnPropertyChanged<T>(Expression<Func<T>> propertyExpresssion)
        {
            var propertyName = GetPropertyName(propertyExpresssion);
            this.OnPropertyChanged(propertyName);
        }

        /// <summary>
        /// Calls the event handler to show that a property has changed
        /// </summary>
        /// <param name="propertyName"></param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            VerifyPropertyName(propertyName);
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Updates the property backing field and calls notifypropertychanged on the VM
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propField"></param>
        /// <param name="newValue"></param>
        /// <param name="propertyName"></param>
        protected void SetProperty<T>(ref T propField, T newValue, [CallerMemberName] string propertyName = null)
        {
            propField = newValue;
            OnPropertyChanged(propertyName);
        }

        /// <summary>
        /// Warns the developer if this Object does not have a public property with
        /// the specified name.
        /// </summary>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public void VerifyPropertyName(String propertyName)
        {
            // Check property exists and we don't have a typo
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                Debug.Fail("Invalid property name: " + propertyName);
            }
        }

        /// <summary>
        /// Used to extract the property name from the expression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyExpresssion"></param>
        /// <returns></returns>
        private string GetPropertyName<T>(Expression<Func<T>> propertyExpresssion)
        {
            if (propertyExpresssion == null)
            {
                throw new ArgumentNullException("propertyExpresssion");
            }

            var memberExpression = propertyExpresssion.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new ArgumentException("The expression is not a member access expression.", "propertyExpresssion");
            }

            var property = memberExpression.Member as PropertyInfo;
            if (property == null)
            {
                throw new ArgumentException("The member access expression does not access a property.", "propertyExpresssion");
            }

            var getMethod = property.GetGetMethod(true);
            if (getMethod.IsStatic)
            {
                throw new ArgumentException("The referenced property is a static property.", "propertyExpresssion");
            }

            return memberExpression.Member.Name;
        }
    }
}
