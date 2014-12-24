using System;
using Hi.UrlRewrite.Templates.Match;
using Sitecore.Data.Items;
using System.Collections.Generic;
using Sitecore.Data.Fields;
using Sitecore.Web.UI.WebControls;

namespace Hi.UrlRewrite.Templates.Conditions
{
    public partial class OutboundMatchItem : CustomItem
    {

        public static readonly string TemplateId = "{55E021F5-E48A-4340-AC93-8861BBE3C104}";

        #region Inherited Base Templates

        private readonly BaseMatchItem _BaseMatchItem;
        public BaseMatchItem MatchIgnoreCaseItem { get { return _BaseMatchItem; } }

        private readonly MatchScopeItem _MatchScopeItem;
        public MatchScopeItem MatchScopeItem { get { return _MatchScopeItem; } }

        private readonly MatchScopeValueItem _MatchScopeValueItem;
        public MatchScopeValueItem MatchScopeValueItem { get { return _MatchScopeValueItem; } }


        #endregion

        #region Boilerplate CustomItem Code

        public OutboundMatchItem(Item innerItem)
            : base(innerItem)
        {
            _BaseMatchItem = new BaseMatchItem(innerItem);
            _MatchScopeItem = new MatchScopeItem(innerItem);
            _MatchScopeValueItem = new MatchScopeValueItem(innerItem);
        }

        public static implicit operator OutboundMatchItem(Item innerItem)
        {
            return innerItem != null ? new OutboundMatchItem(innerItem) : null;
        }

        public static implicit operator Item(OutboundMatchItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        #endregion //Boilerplate CustomItem Code


        #region Field Instance Methods

        #endregion //Field Instance Methods
    }
}