import React, { Component } from 'react';
import PropTypes from 'prop-types';
import Select from 'react-select';

import DropdownIndicator from './DropdownIndicator';
import MenuList from './MenuList';
import Option from './Option';

export default class MultiSelectSearchbox extends Component {
    static propTypes = {
        value: PropTypes.arrayOf(PropTypes.object),
        options: PropTypes.arrayOf(PropTypes.object),
        onChange: PropTypes.func
    };

    static defaultProps = {
        value: [],
        options: [],
        onChange: function () { }
    };

    constructor(props) {
        super(props);

        this.state = {
            isOpen: false
        };
    }

    componentDidMount() {
        document.addEventListener('mousedown', this.handleClickOutside);
    }

    shouldComponentUpdate(nextProps, nextState) {
        if (JSON.stringify(this.props.value) !== JSON.stringify(nextProps.value)
            || JSON.stringify(this.props.options) !== JSON.stringify(nextProps.options)
            || this.state.isOpen !== nextState.isOpen) {
            return true;
        }

        return false;
    }

    componentWillUnmount() {
        document.removeEventListener('mousedown', this.handleClickOutside);
    }

    handleClickOutside = event => {
        if (this.wrapperRef && !this.wrapperRef.contains(event.target)) {
            this.setState({
                isOpen: false
            });
        }
    }

    handleOpen = () => {
        this.setState(state => ({ isOpen: !state.isOpen }));
    }

    handleChange = value => {
        this.props.onChange(value);
    }

    render() {
        return (
            <div className="react-multiselect-container" ref={node => this.wrapperRef = node}>
                <div className={`react-multiselect-content ${this.state.isOpen ? 'is-focused' : ''}`} onClick={this.handleOpen}>
                    <div>{this.props.value.length} item(s) selected</div>
                    <div className="react-multiselect-indicators">
                        <span className="separator" />
                        <span className="icon-chevron-down" />
                    </div>
                </div>

                {
                    this.state.isOpen &&
                    <Select className="react-select-container" classNamePrefix="react-select"
                        value={this.props.value}
                        options={this.props.options}
                        autoFocus
                        menuIsOpen
                        backspaceRemovesValue={false}
                        controlShouldRenderValue={false}
                        isMulti
                        hideSelectedOptions={false}
                        closeMenuOnSelect={false}
                        isSearchable
                        isClearable={false}
                        menuPlacement="auto"
                        placeholder="Search..."
                        components={{ MenuList, Option, DropdownIndicator, IndicatorSeparator: null }}
                        onChange={this.handleChange}
                        styles={{ multiValue: base => ({ ...base, backgroundColor: 'none', border: 'unset' }) }}
                    />
                }
            </div>
        );
    }
}