import React, { Component, Fragment } from 'react';
import ReactDOM from 'react-dom';
import { Async } from 'react-select';
import axios from 'axios';

import { axiosError, resolveClientUrl } from './common-functions';

export default class App extends Component {
    constructor(props) {
        super(props);

        this.state = {
            members: [],
            showSearchbox: false
        };
    }

    componentDidMount() {
        axios.get(resolveClientUrl('common/GetMyMember'))
            .then(response => {
                this.setState({
                    members: response.data
                });
            }).catch(axiosError);
    }

    loadOptions = (inputValue) => {
        if (inputValue.length > 1) {
            return axios.get(resolveClientUrl('common/GetMyMember'), { params: { keyword: inputValue } })
                .then(response => {
                    let options = [];

                    response.data.map(item => {
                        options.push({
                            value: item.employeeId,
                            label: `${item.fullName} (${item.nickName})`
                        });
                    });

                    return options;
                }).catch(axiosError);
        }

        return [];
    }

    handleChange = (selectedOption) => {
        if (selectedOption && selectedOption.value) {
            window.location = resolveClientUrl(`member/details/${selectedOption.value}`);
        }
    }

    handleClick = () => {
        this.setState((prevState, prevProps) => {
            return { showSearchbox: !prevState.showSearchbox };
        });
    }

    render() {
        return (
            <Fragment>
                {
                    this.state.members.length > 0 &&
                    <div className={`search-member-container ${this.state.showSearchbox ? 'focused' : ''}`}>
                        <span className="icon-search" onClick={this.handleClick} />
                        {
                            this.state.showSearchbox &&
                            <Async classNamePrefix="react-select" className="react-select-container"
                                cacheOptions
                                loadOptions={this.loadOptions}
                                defaultOptions
                                placeholder=""
                                components={{ DropdownIndicator: null }}
                                noOptionsMessage={() => "Type nick name or full name to search"}
                                loadingMessage={() => "Type nick name or full name to search"}
                                isFocused={this.state.showSearchbox}
                                isLoading={false}
                                onChange={this.handleChange}
                            />
                        }
                    </div>
                }
            </Fragment>
        );
    }
}

ReactDOM.render(<App />, document.getElementById('search-member-wrapper'));