namespace PassManager.Common
{
    using System.ComponentModel;

    ///<summary>
    ///ViewModelの基本クラス。InotifyPropertyChangedの実装を提供します。
    ///</summary>
    public class ViewModelBase : INotifyPropertyChanged
    {
        ///<summary>
        /// プロパティの変更があったときに発行されます
        ///</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        ///<summary>
        ///PropertyChangedイベントを発行します。
        ///</summary>
        ///<param name="propertyName">プロパティ名</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var h = PropertyChanged;
            if(h != null)
            {
                h(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
