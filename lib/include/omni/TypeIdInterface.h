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

#ifndef OMNI_TYPEIDINTERFACE_H_
#define OMNI_TYPEIDINTERFACE_H_

#include <omni/Id.h>

namespace omni {
  /// Abstract Interface with a single virtual member function which returns
  // type id
  class OMNI_EXPORT TypeIdInterface {
    public:
      virtual ~TypeIdInterface() = default;

      /// Returns type id of object
      virtual Id   getTypeId() const = 0;

      /// Register the inherited class in factory
      virtual void registerInFactory() const = 0;
  };
}

#define OMNI_TYPEID(type_id) \
  BOOSTX_TYPE_ID(omni::Id, type_id, typeId, getTypeId)

#define OMNI_REGISTER_CLASS(FACTORY, CLASS_NAME) \
  OMNI_TYPEID(# CLASS_NAME)                      \
  virtual void registerInFactory() const override {       \
    FACTORY::template reg<CLASS_NAME>();         \
  }

#endif /* OMNI_TYPEIDINTERFACE_H_ */
