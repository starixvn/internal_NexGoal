import React, { Component, Fragment } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';

import { axiosError, resolveClientUrl } from './common-functions';
import getResource from './resourceHelper';

export default class SearchMember extends Component {
    constructor(props) {
        super(props);

        this.state = {
            hasMember: false,
            showSearchbox: false,
            memberOptions: []
        };
    }

    componentDidMount() {
        document.addEventListener('mousedown', this.handleClickOutside);

        axios.get(resolveClientUrl('common/GetMyMember'))
            .then(response => {
                this.setState({
                    hasMember: response.data && response.data.length > 0
                });
            }).catch(axiosError);
    }

    componentWillUnmount() {
        document.removeEventListener('mousedown', this.handleClickOutside);
    }

    handleClickOutside = (event) => {
        if (this.wrapperRef && !this.wrapperRef.contains(event.target) && this.state.showSearchbox && !this.inputRef.value) {
            this.setState({
                showSearchbox: false,
                memberOptions: []
            });
        }
    }

    handleChange = (e) => {
        let value = e.target.value;

        if (value.length > 1) {
            axios.get(resolveClientUrl('common/GetMyMember'), { params: { keyword: value } })
                .then(response => {
                    let options = [];

                    response.data.map(item => {
                        options.push({
                            value: item.employeeId,
                            label: `${item.nickName} (${item.fullName})`
                        });
                    });

                    this.setState({ memberOptions: options });
                }).catch(axiosError);
        } else {
            this.setState({ memberOptions: [] });
        }
    }

    handleClick = () => {
        this.inputRef.focus();

        if (!this.state.showSearchbox) {
            this.setState({ showSearchbox: true });
        }
    }

    handleSelect = (e) => {
        let memberId = e.target.attributes['memberid'].value;
        if (memberId) {
            window.location = resolveClientUrl(`member/details/${memberId}`);
        }
    }

    render() {
        let memberOptions = this.state.memberOptions;

        return (
            <Fragment>
                {
                    this.state.hasMember &&
                    <div ref={node => this.wrapperRef = node}
                        className={`search-member-container ${this.state.showSearchbox ? 'focused' : ''} ${memberOptions.length > 0 ? ' has-member' : ''}`}
                    >
                        <span className="icon-search" onClick={this.handleClick} />
                        <input ref={node => this.inputRef = node}
                            autoFocus
                            placeholder={getResource('NicknameOrFullName')}
                            onChange={this.handleChange}
                        />
                        {
                            memberOptions.length > 0 &&
                            <div className="member-list">
                                {
                                    memberOptions.map(member => {
                                        return (
                                            <div key={member.value}
                                                className="item"
                                                onClick={this.handleSelect}
                                                memberId={member.value}
                                            >
                                                {`${member.label}`}
                                            </div>
                                        );
                                    })
                                }
                            </div>
                        }
                    </div>
                }
            </Fragment>
        );
    }
}

ReactDOM.render(<SearchMember />, document.getElementById('search-member-wrapper'));