﻿using System;
using System.Linq.Expressions;
using System.Reflection;


namespace EasyData
{
    /// <summary>
    /// Builder for entity metadata.
    /// </summary>
    public class MetaEntityBuilder<TEntity> : IMetaEntityBuilder<TEntity> where TEntity : class
    {
        public MetaEntity Entity { get; private set; }

        /// <summary>
        /// Initialize entity builder
        /// </summary>
        public MetaEntityBuilder(MetaEntity entity)
        {
            Entity = entity;
        }

        /// <summary>
        /// Set entity display name.
        /// </summary>
        /// <param name="displayName">Name to set.</param>
        /// <returns>Current instance of the class.</returns>
        public IMetaEntityBuilder<TEntity> SetDisplayName(string displayName)
        {
            Entity.Name = displayName;
            return this;
        }


        //TODO: We should check if really need this SetEnabled procedure

        /// <summary>
        /// Set availability for the entity.
        /// </summary>
        /// <param name="enabled">Enable or not.</param>
        /// <returns>Current instance of the class.</returns>
        public IMetaEntityBuilder<TEntity> SetEnabled(bool enabled)
        {
            if (!enabled) {
                Entity.Parent.SubEntities.Remove(Entity);
            }
            return this;
        }

        /// <summary>
        /// Set entity plural display name.
        /// </summary>
        /// <param name="displayNamePlural">Name to set.</param>
        /// <returns>Current instance of the class.</returns>
        public IMetaEntityBuilder<TEntity> SetDisplayNamePlural(string displayNamePlural)
        {
            Entity.NamePlural = displayNamePlural;
            return this;
        }

        /// <summary>
        /// Set entity description.
        /// </summary>
        /// <param name="description">Description to set.</param>
        /// <returns>Current instance of the class.</returns>
        public IMetaEntityBuilder<TEntity> SetDescription(string description)
        {
            Entity.Description = description;
            return this;
        }

        /// <summary>
        /// Set entity availability for editing.
        /// </summary>
        /// <param name="editable">Editable or not.</param>
        /// <returns>Current instance of the class.</returns>
        public IMetaEntityBuilder<TEntity> SetEditable(bool editable)
        {
            Entity.IsEditable = editable;
            return this;
        }

        /// <summary>
        /// Get entity attribute metadata builder.
        /// </summary>
        /// <param name="propertySelector">Property expression.</param>
        /// <returns>Attribute metadata builder instance.</returns>
        public IMetaEntityAttrBuilder Attribute(Expression<Func<TEntity, object>> propertySelector)
        {
            PropertyInfo propertyInfo;

            if (propertySelector.Body is MemberExpression expression) {
                propertyInfo = (PropertyInfo)expression.Member;
            }
            else {
                var memberExpression = ((UnaryExpression)propertySelector.Body).Operand as MemberExpression;
                propertyInfo = (PropertyInfo)memberExpression.Member;
            }

            var metaAttr = Entity.FindAttribute(attr => attr.PropInfo.Equals(propertyInfo));
            var attributeBuilder = new MetaEntityAttrBuilder(metaAttr);
            return attributeBuilder;
        }
    }

}