import React, { Component } from 'react';
import PropTypes from 'prop-types';

let id = 1;

export default class Multiselect extends Component {
    static propTypes = {
        items: PropTypes.arrayOf(PropTypes.object).isRequired,
        texts: PropTypes.object,
        isSelectedAll: PropTypes.bool,
        maxDisplayItemCount: PropTypes.number,
        hasSearchBox: PropTypes.bool,
        onChange: PropTypes.func
    };

    static defaultProps = {
        items: [],
        texts: {
            all: 'All',
            selectedItemCount: '# item(s) selected',
            placeholder: 'Please select...'
        },
        isSelectedAll: false,
        maxDisplayItemCount: 1,
        hasSearchBox: false,
        onChange: function () {
            // no thing
        }
    };

    constructor(props) {
        super(props);
        this.id = `multiselect-${props.id || id++}`; // NOSONAR
        this.state = {
            items: props.items,
            searchTerm: '',
            selectedText: props.texts.placeholder,
            opened: false
        };
    }

    componentDidMount() {
        document.addEventListener('click', this.handleDocumentClick);
    }

    componentWillUnmount() {
        document.removeEventListener('click', this.handleDocumentClick);
    }

    componentDidUpdate(prevProps) {
        if (this.props.items !== prevProps.items) {
            this.setState({ items: this.props.items });
        }
    }

    handleDocumentClick = event => {
        const clickOutside = this.wrapper && !this.wrapper.contains(event.target);
        if (clickOutside && this.state.opened) {
            this.setState({ opened: false });
        }
    };

    getSelectedText = (props, items) => {
        const { texts, maxDisplayItemCount } = props;
        const selected = items.filter(item => item.checked),
            selectedItemCount = selected.length;
        let text = '';

        if (selectedItemCount === 0) {
            text = texts.placeholder;
        } else if (items.length === selectedItemCount) {
            text = texts.all;
        } else if (selectedItemCount > maxDisplayItemCount) {
            text = texts.selectedItemCount.replace('#', selectedItemCount);
        } else {
            if (selectedItemCount >= 1) {
                text = selected.map(item => item.text).join(', ');
            }
        }

        return text;
    };

    getSelectedItems = items => items.filter(item => item.checked);

    onToggle = () => {
        this.setState(prevState => {
            return { opened: !prevState.opened };
        });
    };

    onSelectAll = event => {
        const items = [...this.state.items];
        const totalItems = items.length;
        for (let i = 0; i < totalItems; i++) {
            if (items[i].visible === undefined || items[i].visible) {
                items[i] = { ...items[i], checked: event.target.checked };
            }
        }

        this.props.onChange && this.props.onChange(this.getSelectedItems(items));
        this.setState({ items });
    };

    onSelect = (item, event) => {
        const items = [...this.state.items];
        const index = items.findIndex(x => x.id === item.id);
        items[index] = { ...items[index], checked: event.target.checked };

        this.props.onChange && this.props.onChange(this.getSelectedItems(items));
        this.setState({ items });
    };

    onSearch = value => {
        const items = [...this.state.items],
            totalItems = items.length;
        for (let i = 0; i < totalItems; i++) {
            items[i] = { ...items[i], visible: items[i].text.toLowerCase().indexOf(value.toLowerCase()) !== -1 };
        }

        this.setState({ searchTerm: value, items });
    };

    renderSearchBox = () => {
        if (!this.props.hasSearchBox || this.state.items.length === 0) {
            return null;
        }

        return (
            <div className="react-multiselect-search-box">
                <input autoFocus onChange={event => this.onSearch(event.target.value)}
                    ref={element => this.searchInputBox = element}
                    value={this.state.searchTerm} type="text" placeholder="Search data" />
                <span className="clear-indicator" onClick={() => this.onSearch('')}>
                    <svg height="20" width="20" viewBox="0 0 20 20" focusable="false">
                        <path d="M14.348 14.849c-0.469 0.469-1.229 0.469-1.697 0l-2.651-3.030-2.651 3.029c-0.469 0.469-1.229 0.469-1.697 0-0.469-0.469-0.469-1.229
                            0-1.697l2.758-3.15-2.759-3.152c-0.469-0.469-0.469-1.228 0-1.697s1.228-0.469 1.697 0l2.652 3.031
                            2.651-3.031c0.469-0.469 1.228-0.469 1.697 0s0.469 1.229 0 1.697l-2.758 3.152 2.758 3.15c0.469 0.469 0.469 1.229 0 1.698z"
                        />
                    </svg>
                </span>
            </div>
        );
    };

    render() {
        const { items, opened } = this.state;
        const selected = items.filter(item => item.checked);
        const selectedText = this.getSelectedText(this.props, items);

        return (
            <div id={this.id} className="react-multiselect" ref={element => this.wrapper = element}
                style={{ width: this.props.width ? this.props.width : '100%' }}>
                <div className="react-multiselect-dropdown-button" onClick={this.onToggle}>
                    <span className="selected-text">{selectedText}</span>
                    <span className="dropdown-indicator">
                        <svg height="20" width="20" viewBox="0 0 20 20" focusable="false">
                            <path d="M4.516 7.548c0.436-0.446 1.043-0.481 1.576 0l3.908 3.747 3.908-3.747c0.533-0.481 1.141-0.446 1.574 0
                                0.436 0.445 0.408 1.197 0 1.615-0.406 0.418-4.695 4.502-4.695 4.502-0.217 0.223-0.502 0.335-0.787
                                0.335s-0.57-0.112-0.789-0.335c0 0-4.287-4.084-4.695-4.502s-0.436-1.17 0-1.615z"
                            />
                        </svg>
                    </span>
                </div>
                <div className={`react-multiselect-dropdown-list ${opened ? '' : 'hidden'}`}>
                    {this.renderSearchBox()}
                    {
                        items.filter(item => item.visible === undefined || item.visible).length > 0 &&
                        <div className="react-multiselect-select-all">
                            <div className="checkbox-wrapper">
                                <div>
                                    <input type="checkbox" id={`${this.id}-selectAll`}
                                        checked={items.length === selected.length}
                                        onChange={this.onSelectAll}
                                    />
                                    <label htmlFor={`${this.id}-selectAll`}> Sellect all</label>
                                </div>
                            </div>
                        </div>
                    }
                    {
                        items.length > 0 &&
                        <div className="react-multiselect-items">
                            {
                                items.map(item => {
                                    return item.visible === undefined || item.visible ?
                                        <ul key={item.id}>
                                            <li className="react-multiselect-item">
                                                <div className="checkbox-wrapper">
                                                    <div>
                                                        <input type="checkbox" id={`${this.id}-${item.id}`}
                                                            checked={item.checked}
                                                            onChange={event => this.onSelect(item, event)}
                                                        />
                                                        <label htmlFor={`${this.id}-${item.id}`}> {item.text}</label>
                                                    </div>
                                                </div>
                                            </li>
                                        </ul> : null;
                                })
                            }
                        </div>
                    }
                </div>
            </div>
        );
    }
}