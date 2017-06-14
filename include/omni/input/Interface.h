/* Copyright (c) 2014-2015 "Omnidome" by Michael Winkelmann
 * Dome Mapping Projection Software (http://omnido.me).
 * Omnidome was created by Michael Winkelmann aka Wilston Oreo (@WilstonOreo)
 *
 * This file is part of Omnidome.
 *
 * Omnidome is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as
 * published by the Free Software Foundation, either version 3 of the
 * License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Affero General Public License for more details.
 * You should have received a copy of the GNU Affero General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */

#ifndef OMNI_INPUT_INTERFACE_H_
#define OMNI_INPUT_INTERFACE_H_

#include <memory>
#include <set>
#include <map>
#include <functional>
#include <QOpenGLTexture>
#include <omni/exception.h>
#include <omni/PluginInfo.h>
#include <omni/serialization/PropertyMapSerializer.h>
#include <omni/mapping/Interface.h>

namespace omni {
  namespace input {

    /// Generic input interface
    class Interface :
      public QObject,
      public TypeIdInterface,
      public PropertyMapSerializer {
        Q_OBJECT
        Q_PROPERTY(int width READ width CONSTANT)
        Q_PROPERTY(int height READ height CONSTANT)
      public:
        friend class input::List;

        typedef Interface                                     interface_type;
        typedef std::vector<Interface const*>                 inputlist_type;
        typedef std::set<QString> categoryset_type;

        Interface() {}

        /// Virtual destructor
        virtual ~Interface() {}

        /// An input must return an OpenGL texture ID
        virtual GLuint textureId() const = 0;

        /// Update interface
        inline virtual void update() {}

        /// An input must return width and height information
        virtual QSize size() const = 0;

        /// Return width from size
        inline int width() const
        {
          return size().width();
        }

        /// Return height from size
        inline int height() const
        {
          return size().height();
        }

        /// Optional info text
        virtual QString infoText() const {
          return QString();
        }

#ifndef OMNI_DEAMON
        /// Make new parameter widget
        virtual QWidget* widget();
#endif

        /**@brief Returns true if this input can be added
           @detail E.g., an input can be added after an initial settings dialog
                  was approved or it has valid settings.
           @return Flag if input can be added
         **/
        inline virtual bool canAdd() {
          return true;
        }

        /// Serialize to property map
        virtual void          toPropertyMap(PropertyMap&) const {}
        /// Deserialize from property map
        virtual void          fromPropertyMap(PropertyMap const&) {}
      signals:
        void updated();

      private:
        inline virtual void activate() {
        }

        inline virtual void deactivate() {
        }
    };

    /// Input Factory typedef
    typedef AbstractFactory<Interface> Factory;
  }

  typedef input::Interface Input;
  typedef input::Factory   InputFactory;
}

#define OMNI_INPUT_INTERFACE_IID "org.omnidome.input.Interface"

Q_DECLARE_INTERFACE(omni::input::Interface, OMNI_INPUT_INTERFACE_IID)

#define OMNI_INPUT_PLUGIN_DECL                    \
  Q_OBJECT                                        \
  Q_PLUGIN_METADATA(IID OMNI_INPUT_INTERFACE_IID) \
  Q_INTERFACES(omni::input::Interface)            \
  OMNI_PLUGIN_TYPE("Input")

#endif /* OMNI_INPUT_INTERFACE_H_ */
