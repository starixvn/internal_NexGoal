import React, { Component, Fragment } from 'react';

export default class Row extends Component {
    constructor(props) {
        super(props);

        this.state = {
            expand: true
        };
    }

    toggleExpand = () => this.setState({ expand: !this.state.expand })

    handleAddMember = pm => this.props.onAddMember(pm)

    handleRemoveMember = member => this.props.onRemoveMember(member)

    renderMember = (member) => {
        return (
            <tr className={this.state.expand ? '' : 'hidden'} key={member.employeeId}>
                <td>{member.nickName} ({member.fullName})</td>
                <td>{member.jobTitle}</td>
                <td className='text-center'>
                    <a className="icon-delete" onClick={this.handleRemoveMember.bind(this, member)} title="Remove" />
                </td>
            </tr>
        );
    }

    render() {
        let data = this.props.data;
        let styleCenter = {
          verticalAlign: 'middle'
        };

        return (
            <Fragment >
                <tr>
                    <td colSpan={3}>
                        <span style={styleCenter}>{data.pm.nickName} ({data.members.length} members)</span>
                        <a className="icon-plus" title="Add" onClick={this.handleAddMember.bind(this, data.pm)} />
                        <a className={`icon-chevron-down ${this.state.expand ? 'collapsed' : 'expanded'}`}
                            title="Collapse/Expand"
                            onClick={this.toggleExpand}
                        />
                    </td>
                </tr>
                {data.members.map(this.renderMember)}
            </Fragment>
        );
    }
}