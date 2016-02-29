/* Copyright (c) 2014-2016 "Omnidome" by cr8tr
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

#include <cmath>
#include <omni/proj/ChannelCorrection.h>

namespace omni {
    namespace proj {
        double ChannelCorrection::gamma() const {
            return gamma_;
        }

        void ChannelCorrection::setGamma(double _gamma) {
            gamma_ = qBound(-1.0, _gamma, 1.0);
        }

        double ChannelCorrection::brightness() const {
            return brightness_;
        }

        double ChannelCorrection::contrast(double v) const {
            double _c = (contrast() <= 0.0) ?
                contrast()*multiplier() + 1.0 :
                (1.0 / (1.0 - contrast()*multiplier()));
            return ((v - 0.5) * std::max(_c, 0.0)) + 0.5;
        }

        double ChannelCorrection::brightness(double v) const {
        	return std::max(v + brightness() * multiplier(),0.0);
        }

        double ChannelCorrection::gamma(double v) const {
            double g = 0.0;
            if (gamma() >= 0.0) {
            	g = 1.0 / (1.0 + gamma() * multiplier());
            } else {
            	g = (1.0 - gamma() * multiplier());
            }
            return std::pow(v,g*g*g*g);
        }

        void ChannelCorrection::setBrightness(double _brightness) {
            brightness_ = qBound(-1.0, _brightness, 1.0);
        }

        double ChannelCorrection::contrast() const {
            return contrast_;
        }

        void ChannelCorrection::setContrast(double _contrast) {
            contrast_ = qBound(-1.0, _contrast, 1.0);
        }

        double ChannelCorrection::multiplier() const {
            return multiplier_;
        }

        void ChannelCorrection::setMultiplier(double _multiplier) {
            multiplier_ = qBound(0.0, _multiplier, 1.0);
        }

        double ChannelCorrection::corrected(double _value) const {
            return brightness(contrast(gamma(_value)));
        }
    }
}
