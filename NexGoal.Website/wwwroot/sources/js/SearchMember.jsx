import React, { Component, Fragment } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';

import { axiosError, resolveClientUrl } from './common-functions';

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
        axios.get(resolveClientUrl('common/GetMyMember'))
            .then(response => {
                this.setState({
                    hasMember: response.data && response.data.length > 0
                });
            }).catch(axiosError);
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
        }

        this.setState({ memberOptions: [] });
    }

    handleClick = () => {
        this.setState((prevState, prevProps) => {
            return { showSearchbox: !prevState.showSearchbox };
        });
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
                    <div className={`search-member-container ${this.state.showSearchbox ? 'focused' : ''}`}>
                        <span className="icon-search" onClick={this.handleClick} />
                        {/* {
                            this.state.showSearchbox &&
                            <input
                                autoFocus
                                placeholder="nickname or full name"
                                onChange={this.handleChange}
                            />
                        } */}
                        <input
                                autoFocus
                                placeholder="nickname or full name"
                                onChange={this.handleChange}
                            />
                    </div>
                }
                {
                    memberOptions.length > 0 &&
                    <div className="member-list">
                        {
                            memberOptions.map(member => {
                                return <div className="item" onClick={this.handleSelect} memberId={member.value}>{`${member.label}`}</div>;
                            })
                        }
                    </div>
                }
            </Fragment>
        );
    }
}

ReactDOM.render(<SearchMember />, document.getElementById('search-member-wrapper'));