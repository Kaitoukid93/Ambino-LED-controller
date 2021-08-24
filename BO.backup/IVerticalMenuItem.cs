using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{

    public enum ViewType
    {
        Dashboard,
        AppSettings,
        About
    }
   public interface IVerticalMenuItem
    {
        
         bool IsActive { get; set; }
     
        
         string Text { get; set; }
       
        
         string Images { get; set; }
    
       
         bool IsVisible { get; set; }
       
     
         ViewType Type { get; set; }
    
    
   
}
}
